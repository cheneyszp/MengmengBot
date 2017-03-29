using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using MengmengBot;
using MengmengBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Luis;
using System.Collections.Generic;
using System.Text;
using StateBot;

namespace MengmengBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        
        [System.Web.Http.AcceptVerbs("GET","POST")]
        [System.Web.Http.HttpPost]
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            { 
                await Conversation.SendAsync(activity, () => new MengmengDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }


        [LuisModel("YOU_LUIS_ID", "YOU_LUIS_KEY")]
        [Serializable]
        public class MengmengDialog : LuisDialog<object>
        {
            private bool userWelcomed;

            String weAreTalking = "";
            String userName = "";

            public MengmengDialog()
            {
            }
            public MengmengDialog(ILuisService service)
                : base(service)
            {
            }

            [LuisIntent("")]
            public async Task None(IDialogContext context, LuisResult result)
            {
                context.UserData.TryGetValue(ContextConstants.TopicKey, out weAreTalking);
                String entityInNone = "";
                string replyString = "";
                if (weAreTalking.Equals("查询天气") && TryToFindLocation(result, out entityInNone))
                {
                    replyString = await GetWeather(entityInNone);
                    await context.PostAsync((userName.Equals("") ? "" : $"报告{userName}，") + replyString);
                    context.Wait(MessageReceived);
                }
                else if (weAreTalking.Equals("查询股票") && TryToFindNunberOfStock(result, out entityInNone))
                {
                    replyString = await GetStock(entityInNone);
                    await context.PostAsync((userName.Equals("") ? "" : $"报告{userName}，") + replyString);
                    context.Wait(MessageReceived);
                }
                else
                {
                    replyString = $"萌萌不知道你在说什么，面壁去。。。我现在只会查询股票和查询天气。。T_T" + string.Join(", ", result.Intents.Select(i => i.Intent));
                    await context.PostAsync((userName.Equals("") ? "" : $"报告{userName}，") + replyString);
                    context.Wait(MessageReceived);
                }
            }

            [LuisIntent("问候")]
            public async Task SayHello(IDialogContext context, LuisResult result)
            {
                weAreTalking = "问候";
                context.UserData.SetValue(ContextConstants.TopicKey, weAreTalking);
                if (!context.UserData.TryGetValue(ContextConstants.UserNameKey, out userName))
                {
                    string welmessage = $"你好，我是萌萌，你的最亲密的伴侣。我将陪你哭、陪你笑、陪你看天上的云卷与舒，陪你去到天涯海角，陪你听你最爱的音乐，陪你留下你的最美丽的身影，不管刮风下雨，不管生老病死，我都在你身边，关心你，提醒你，爱着你。";
                    await context.PostAsync(welmessage);
                    PromptDialog.Text(context, this.AfterEnterNamePrompt, "对了，萌萌该怎么称呼您呢?");
                    return;
                }
                string message = $"{userName} 你好呀，萌萌想你啦！";
                await context.PostAsync(message);
                context.Wait(MessageReceived);
            }

            private async Task AfterEnterNamePrompt(IDialogContext context, IAwaitable<string> result)
            {
                try
                {
                    var userName = await result;
                    this.userWelcomed = true;

                    await context.PostAsync($"{userName} 你好! 萌萌给你请安啦！");

                    context.UserData.SetValue(ContextConstants.UserNameKey, userName);
                }
                catch (TooManyAttemptsException)
                {
                }

                context.Wait(MessageReceived);
            }


            public bool TryToFindLocation(LuisResult result, out String location)
            {
                location = "";
                EntityRecommendation title;
                if (result.TryFindEntity("地点", out title))
                {
                    location = title.Entity;
                }
                else
                {
                    location = "";
                }
                return !location.Equals("");
            }

            [LuisIntent("查询天气")]
            public async Task QueryWeather(IDialogContext context, LuisResult result)
            {
                weAreTalking = "查询天气";
                context.UserData.SetValue(ContextConstants.TopicKey, weAreTalking);
                string location = "";
                string replyString = "";
                if (TryToFindLocation(result, out location))
                {
                    replyString = await GetWeather(location);
                    await context.PostAsync((userName.Equals("") ? "" : $"报告{userName}，") + replyString);
                    context.Wait(MessageReceived);
                }
                else
                {
                    await context.PostAsync((userName.Equals("") ? "亲" : $"{userName}") + "你要查询哪个地方的天气信息呢，快把城市的名字发给我吧");
                    context.Wait(AfterEnterLocation);
                }
            }

            public async Task AfterEnterLocation(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var message = await argument;
                String replyString = await GetWeather(message.Text);
                await context.PostAsync(replyString);
                context.Wait(MessageReceived);
            }

            private async Task<string> GetWeather(string cityname)
            {
                WeatherData weatherdata = await MengmengBotTask.GetWeatherAsync(cityname);
                if (weatherdata == null || weatherdata.HeWeatherdataservice30 == null)
                {
                    return string.Format("呃。。。萌萌不知道\"{0}\"这个城市的天气信息", cityname);
                }
                else
                {
                    HeweatherDataService30[] weatherServices = weatherdata.HeWeatherdataservice30;
                    if (weatherServices.Length <= 0) return string.Format("呃。。。萌萌不知道\"{0}\"这个城市的天气信息", cityname);
                    Basic cityinfo = weatherServices[0].basic;
                    if (cityinfo == null) return string.Format("呃。。。萌萌目测\"{0}\"这个应该不是一个城市的名字。。不然我咋不知道呢。。。", cityname);
                    String cityinfoString = "城市信息：" + cityinfo.city + "\r\n"
                        + "更新时间:" + cityinfo.update.loc + "\r\n"
                        + "经纬度:" + cityinfo.lat + "," + cityinfo.lon + "\r\n";
                    Aqi cityAirInfo = weatherServices[0].aqi;
                    String airInfoString = "空气质量指数：" + cityAirInfo.city.aqi + "\r\n"
                        + "PM2.5 1小时平均值：" + cityAirInfo.city.pm25 + "(ug/m³)\r\n"
                        + "PM10 1小时平均值：" + cityAirInfo.city.pm10 + "(ug/m³)\r\n"
                        + "二氧化硫1小时平均值：" + cityAirInfo.city.so2 + "(ug/m³)\r\n"
                        + "二氧化氮1小时平均值：" + cityAirInfo.city.no2 + "(ug/m³)\r\n"
                        + "一氧化碳1小时平均值：" + cityAirInfo.city.co + "(ug/m³)\r\n";

                    Suggestion citySuggestion = weatherServices[0].suggestion;
                    String suggestionString = "生活指数：" + "\r\n"
                        + "穿衣指数：" + citySuggestion.drsg.txt + "\r\n"
                        + "紫外线指数：" + citySuggestion.uv.txt + "\r\n"
                        + "舒适度指数：" + citySuggestion.comf.txt + "\r\n"
                        + "旅游指数：" + citySuggestion.trav.txt + "\r\n"
                        + "感冒指数：" + citySuggestion.flu.txt + "\r\n";

                    Daily_Forecast[] cityDailyForecast = weatherServices[0].daily_forecast;
                    Now cityNowStatus = weatherServices[0].now;
                    String nowStatusString = "天气实况：" + "\r\n"
                        + "当前温度(摄氏度)：" + cityNowStatus.tmp + "\r\n"
                        + "体感温度：" + cityNowStatus.fl + "\r\n"
                        + "风速：" + cityNowStatus.wind.spd + "(Kmph)\r\n"
                        + "湿度：" + cityNowStatus.hum + "(%)\r\n"
                        + "能见度：" + cityNowStatus.vis + "(km)\r\n";

                    return string.Format("现在{0}天气实况：\r\n{1}", cityname, cityinfoString + nowStatusString + airInfoString + suggestionString);
                }
            }

            private async Task<string> GetStock(string StockSymbol)
            {
                double? dblStockValue = await MengmengBotTask.GetStockRateAsync(StockSymbol);
                if (dblStockValue == null)
                {
                    return string.Format("呃。。。\"{0}\"这个貌似不是股票代码呢", StockSymbol);
                }
                else
                {
                    return $"{StockSymbol}这个股票现在的价格是{dblStockValue}美元啦！";
                }
            }

            public bool TryToFindNunberOfStock(LuisResult result, out string stockcode)
            {
                stockcode = "";
                EntityRecommendation title;
                if (result.TryFindEntity("股票代码", out title))
                {
                    stockcode = title.Entity;
                }
                else
                {
                    stockcode = "";
                }
                return !stockcode.Equals("");
            }

            [LuisIntent("查询股票")]
            public async Task QueryStock(IDialogContext context, LuisResult result)
            {
                weAreTalking = "查询股票";
                context.UserData.SetValue(ContextConstants.TopicKey, weAreTalking);
                string stockcode = "";
                string replyString = "";
                if (TryToFindNunberOfStock(result, out stockcode))
                {
                    replyString = await GetStock(stockcode);
                    await context.PostAsync((userName.Equals("") ? "" : $"{userName}啊，") + replyString);
                    context.Wait(MessageReceived);
                }
                else
                {
                    await context.PostAsync((userName.Equals("") ? "亲" : $"{userName}，") + "你要查询哪只股票的信息呢，麻烦把股票代码发给我吧");
                    context.Wait(AfterEnterStockNumber);
                }
            }

            public async Task AfterEnterStockNumber(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var message = await argument;
                String replyString = await GetStock(message.Text);
                await context.PostAsync((userName.Equals("") ? "" : $"{userName}{userName}，") + replyString);
                context.Wait(MessageReceived);
            }

        }
    }
}
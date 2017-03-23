# 萌萌机器人 #

这是Bot framework的一个简单示例，请参考文章：
http://www.cnblogs.com/rocsheh/p/5846009.html 

## 配置工作 ##

从Bot Framework申请好bot，打开Web.config文件，配置好以下内容。

```html
<add key="BotId" value="YOU_BOT_ID" />
<add key="MicrosoftAppId" value="YOU_APP_ID" />
<add key="MicrosoftAppPassword" value="YOU_APP_PASSWORD" />
```

在Bot Framework网站上channel上的web打开，打开default.htm文件，把web的密钥配置到以下地方。

```html
<body style="font-family:'Segoe UI'">
<iframe name="myframe" scrolling="auto" width="100%" height="100%" onload="document.all['myframe'].style.height=myframe.document.body.scrollHeight" 
 src="https://webchat.botframework.com/embed/mengmeng?s=YOU_WEB_KEY" style="height: 502px; max-height: 502px;"></iframe>
</body>
```

从luis.ai上申请设置好LUIS，打开MessagesController.cs文件，配置好LUIS的id和key。

```h
 [LuisModel("YOU_LUIS_ID", "YOU_LUIS_KEY")]
```

打开MengmengBotTask.cs文件，配置好天气的key。查询天气的key可以从这里申请：http://www.heweather.com 

```JAVA
string ServiceURL = $"https://api.heweather.com/x3/weather?city={city}&key=YOUR_WEATHER_KEY";
```


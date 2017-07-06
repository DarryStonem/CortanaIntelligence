using BotDemo.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotDemo.Dialogs
{
    [LuisModel("c28272e1-c60b-48c1-963a-5b997b639a11", "af5d8ba1fe964eb79cb70a661f25d9bd", domain: "westcentralus.api.cognitive.microsoft.com", Staging = true)]
    [Serializable]
    public class BookFlightDialog : LuisDialog<object>
    {
        private readonly Dictionary<string, FlightsModel> alarmByWhat = new Dictionary<string, FlightsModel>();

        public const string DefaultAlarmWhat = "default";

    }
}
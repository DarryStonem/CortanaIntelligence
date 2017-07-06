using BotDemo.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Humanizer;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.FormFlow;

namespace BotDemo.Dialogs
{
    // Spanish
    [LuisModel("c28272e1-c60b-48c1-963a-5b997b639a11", "af5d8ba1fe964eb79cb70a661f25d9bd", domain: "westcentralus.api.cognitive.microsoft.com", Staging = false)]

    //English
    //[LuisModel("f60a401d-1299-44f9-807f-42e56d1222cb", "af5d8ba1fe964eb79cb70a661f25d9bd", domain: "westcentralus.api.cognitive.microsoft.com", Staging = false)]

    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        private const string EntityLocation = "Location";

        private const string EntityDateTime = "datetime";

        private IList<string> airlinesOptions = new List<string> { "Aeropato", "VuelaGDL", "Mexican Airlines", "VivaAeroprecio", "El Gran Avion"};

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("BookFlight")]
        public async Task Search(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Estamos analizando tu mensaje: '{message.Text}'...");

            if(result.Entities.Count == 0)
                await context.PostAsync("Lo sentimos, no entendimos tu mensaje");

            EntityRecommendation hotelEntityRecommendation;

            if (result.TryFindEntity(EntityLocation, out hotelEntityRecommendation))
            {
                await context.PostAsync($"Buscando vuelos a '{hotelEntityRecommendation.Entity}'...");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                for (int i = 0; i < 5; i++)
                {
                    var random = new Random(i);
                    var airline = this.airlinesOptions[random.Next(0, this.airlinesOptions.Count - 1)];

                    HeroCard heroCard = new HeroCard()
                    {
                        Title = airline,
                        Subtitle = $"Viaja a {hotelEntityRecommendation.Entity} por {airline} desde $100.",
                        Images = new List<CardImage>()
                        {
                            new CardImage() { Url = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Vuela+con+{airline}&w=500&h=260" }
                        },
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=flights+to+" + HttpUtility.UrlEncode($"{hotelEntityRecommendation.Entity} with {airline}")
                            }
                        }
                    };

                    resultMessage.Attachments.Add(heroCard.ToAttachment());
                }

                await context.PostAsync(resultMessage);
            }

            context.Wait(this.MessageReceived);
        }   
    }
}
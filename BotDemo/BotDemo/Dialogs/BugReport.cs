using BotDemo.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System;
using System.Threading.Tasks;

namespace BotDemo.Flows
{
    [Serializable]
    public class BugReport
    {
        [Describe("Type de ticket")]
        public TicketType TicketType { get; set; }

        [Describe("Sévérité")]
        [Prompt("Comment évaluez vous l'importance de votre demande? {||}", ChoiceStyle = ChoiceStyleOptions.Inline)]
        public TicketCriticity TicketCriticity { get; set; }

        [Prompt("Donner un titre à votre demande")]
        [Describe("Titre")]
        public string Title { get; set; }

        [Describe("Description de la demande")]
        public string Description { get; set; } 

        public static IForm<BugReport> BuildForm()
        {
            OnCompletionAsyncDelegate<BugReport> processTicket = async (context, state) =>
            {
                TicketManager.CreateTicket(state);
                await context.PostAsync("Votre demande a bien été enregistrée.\n\nElle sera traitée dans les meilleurs délais.");
            };


            return new FormBuilder<BugReport>()
                          .Field(nameof(TicketType))
                          .Field(nameof(Title))
                          .Field(new FieldReflector<BugReport>(nameof(Description))
                              .SetDefine((state, field) =>
                              {
                                  string prompt;
                                  switch (state.TicketType)
                                  {
                                      case TicketType.NewFeature:
                                          prompt = "Décrivez précisément la fonctionnalité souhaitée";
                                          break;
                                      case TicketType.Improvement:
                                          prompt = "Dites moi en détail ce qu'il faut améliorer";
                                          break;
                                      case TicketType.Bug:
                                      default:
                                          prompt = "Décrivez précisément le bug et les étapes pour les reproduire";
                                          break;
                                  }
                                  field.SetPrompt(new PromptAttribute(prompt));
                                  return Task.FromResult(true);
                              }))
                          .Field(nameof(TicketCriticity))
                          .Confirm("Voici votre demande, voulez vous la poster ? {*}")
                          .OnCompletion(processTicket)
                          .Build(); 
        }   
    }

    public enum TicketType
    {
        [Describe("Rapport de bug")]
        [Terms("Rapport de bug", "bug", "erreur", "problème")]
        Bug = 1,
        [Describe("Demande de nouvelle fonctionnalité")]
        [Terms("Demande de nouvelle fonctionnalité", "nouvelle fonctionnalité", "fonctionnalité", "nouveauté")]
        NewFeature,
        [Describe("Demande d'amélioration de l'existant")]
        Improvement
    }

    public enum TicketCriticity
    {
        [Describe("Vraiment pas prioritaire")]
        [Terms("Vraiment pas prioritaire")]
        Lowest = 1,
        [Describe("Peu prioritaire")]
        [Terms("Pas prioritaire")]
        Low,
        [Describe("Intermédiaire")]
        [Terms("Intermédiaire")]
        Medium,
        [Describe("prioritaire")]
        [Terms("prioritaire")]
        High,
        [Describe("Très prioritaire")]
        [Terms("Très prioritaire")]
        Highest
    }
}
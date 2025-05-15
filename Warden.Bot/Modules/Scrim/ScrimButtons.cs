namespace Warden.Bot.Modules.Scrim;

public class ScrimButtons : ComponentInteractionModule<ButtonInteractionContext>
{
    [ComponentInteraction("join-scrim")]
    public async Task<string> JoinScrim () => "You clicked a button!";
}
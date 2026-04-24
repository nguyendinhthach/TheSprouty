// ──────────────────────────────────────────────
// TheSprouty | NPC/Cow/LightCowAnimator.cs
// Provides LightCow-specific animation clip names.
// For Brown/Pink/etc. cows: create similar subclasses.
// ──────────────────────────────────────────────
public class LightCowAnimator : CowAnimator
{
    protected override string AnimBlinkIdle      => "LightCow_BlinkIdle";
    protected override string AnimTailWagIdle    => "LightCow_TailWagIdle";
    protected override string AnimWander         => "LightCow_Wander";
    protected override string AnimLayDown        => "LightCow_LayDown";
    protected override string AnimSitBlinkIdle   => "LightCow_SitBlinkIdle";
    protected override string AnimSitTailWagIdle => "LightCow_SitTailWagIdle";
    protected override string AnimSleep          => "LightCow_Sleep";
    protected override string AnimGetUp          => "LightCow_GetUp";
    protected override string AnimSniff          => "LightCow_Sniff";
    protected override string AnimGraze          => "LightCow_Graze";
    protected override string AnimHappy          => "LightCow_Happy";
}

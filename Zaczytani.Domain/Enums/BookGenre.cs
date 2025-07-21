using System.ComponentModel;

namespace Zaczytani.Domain.Enums;

public enum BookGenre
{
    // Gatunki fikcji
    [Description("Literatura ogólna")]
    Fiction,
    [Description("Romans")]
    Romance,
    [Description("Kryminał i sensacja")]
    Mystery,
    [Description("Thriller")]
    Thriller,
    [Description("Horror")]
    Horror,
    [Description("Fantasy")]
    Fantasy,
    [Description("Science fiction")]
    ScienceFiction,
    [Description("Powieść historyczna")]
    HistoricalFiction,
    [Description("Literatura młodzieżowa")]
    YoungAdult,
    [Description("Powieść przygodowa")]
    Adventure,
    [Description("Komedia")]
    Comedy,
    [Description("Dramat")]
    Drama,

    // Gatunki non-fiction
    [Description("Biografie i autobiografie")]
    Biography,
    [Description("Pamiętniki i wspomnienia")]
    Memoir,
    [Description("Poradniki")]
    SelfHelp,
    [Description("Książki historyczne")]
    History,
    [Description("Nauka i technologia")]
    Science,
    [Description("Filozofia")]
    Philosophy,
    [Description("Religia i duchowość")]
    Religion,
    [Description("Literatura podróżnicza")]
    Travel,
    [Description("Polityka i społeczeństwo")]
    Politics,
    [Description("Biznes i zarządzanie")]
    Business,
    [Description("Sztuka i design")]
    Art,
    [Description("Psychologia")]
    Psychology,
    [Description("Książki edukacyjne i akademickie")]
    Education
}
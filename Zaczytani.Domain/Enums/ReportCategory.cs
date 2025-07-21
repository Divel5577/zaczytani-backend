using System.ComponentModel;

namespace Zaczytani.Domain.Enums;

public enum ReportCategory
{
    [Description("Spam")]
    Spam = 0,

    [Description("Mowa nienawiści")]
    HateSpeech = 1,

    [Description("Nieodpowiednie treści")]
    InappropriateContent = 2,

    [Description("Fałszywe informacje")]
    FalseInformation = 3,

    [Description("Plagiat")]
    Plagiarism = 4,

    [Description("Trolling")]
    Trolling = 5,

    [Description("Nie na temat")]
    OffTopic = 6,

    [Description("Naruszenie praw autorskich")]
    CopyrightInfringement = 7,

    [Description("Prywatne informacje")]
    PrivateInformation = 8,

    [Description("Wprowadzenie w błąd")]
    MisleadingContent = 9,

    [Description("Brak wartości merytorycznej")]
    LackOfSubstance = 10,
}

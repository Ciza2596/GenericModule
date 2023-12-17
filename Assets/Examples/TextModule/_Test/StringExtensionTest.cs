using System.Collections.Generic;
using CizaTextModule;
using NUnit.Framework;

public class StringExtensionTest
{
    public const string ConfirmKey = "ConfirmKey";
    public const string CancelKey = "CancelKey";

    public const string ConfirmValue = "ConfirmValue";
    public const string CancelValue = "CancelValue";

    public static readonly Dictionary<string, string> TextMapByKey = new Dictionary<string, string>() { { ConfirmKey, ConfirmValue }, { CancelKey, CancelValue } };

    public static readonly string ControllerTextKeyString = $"<ControllerTextKey=\"{ConfirmKey}\"> <ControllerTextKey=\"{CancelKey}\">";
    public static readonly string ControllerTextKeyString_Is_Null = null;
    public static readonly string ControllerText = $"{ConfirmValue} {CancelValue}";

    public static readonly string LocaleTextKeyString = $"<LocaleTextKey=\"{ConfirmKey}\"> <LocaleTextKey=\"{CancelKey}\">";
    public static readonly string LocaleTextKeyString_Is_Null = null;
    public static readonly string LocaleText = $"{ConfirmValue} {CancelValue}";

    [Test]
    public void _01_GetControllerTextKeys()
    {
        // act
        var controllerTextKeys = ControllerTextKeyString.GetControllerTextKeys();

        // assert
        Assert.AreEqual(ConfirmKey, controllerTextKeys[0], "Index: 0 should be 'Confirm'.");
        Assert.AreEqual(CancelKey, controllerTextKeys[1], "Index: 1 should be 'Cancel'.");
    }

    [Test]
    public void _02_GetControllerTextKeys_If_String_Is_Null()
    {
        // act
        var controllerTextKeys = ControllerTextKeyString_Is_Null.GetControllerTextKeys();

        // assert
        Assert.AreEqual(0, controllerTextKeys.Length, "Length should be zero.");
    }

    [Test]
    public void _03_GetLocaleTextKeys()
    {
        // act
        var localeTextKeys = LocaleTextKeyString.GetLocaleTextKeys();

        // assert
        Assert.AreEqual(ConfirmKey, localeTextKeys[0], "Index: 0 should be 'Confirm'.");
        Assert.AreEqual(CancelKey, localeTextKeys[1], "Index: 1 should be 'Cancel'.");
    }

    [Test]
    public void _04_GetLocaleTextKeys_If_String_Is_Null()
    {
        // act
        var localeTextKeys = LocaleTextKeyString_Is_Null.GetLocaleTextKeys();

        // assert
        Assert.AreEqual(0, localeTextKeys.Length, "Length should be zero.");
    }

    [Test]
    public void _05_ReplaceByControllerTextKey()
    {
        // act
        var text = ControllerTextKeyString.ReplaceByControllerTextKey(TextMapByKey, string.Empty, string.Empty);

        // assert
        Assert.AreEqual(ControllerText, text, $"Text should be {ControllerText}.");
    }

    [Test]
    public void _06_ReplaceByLocaleTextKey()
    {
        // act
        var text = LocaleTextKeyString.ReplaceByLocaleTextKey(TextMapByKey, string.Empty, string.Empty);

        // assert
        Assert.AreEqual(LocaleText, text, $"Text should be {LocaleText}.");
    }
}
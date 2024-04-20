using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace AccountingBlueBook.Localization
{
    public static class AccountingBlueBookLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(AccountingBlueBookConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(AccountingBlueBookLocalizationConfigurer).GetAssembly(),
                        "AccountingBlueBook.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}

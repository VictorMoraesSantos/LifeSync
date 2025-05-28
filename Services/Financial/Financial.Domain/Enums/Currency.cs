namespace Financial.Domain.Enums
{
    public enum Currency
    {
        USD,    // United States Dollar
        EUR,    // Euro
        BRL,    // Brazilian Real
        GBP,    // British Pound Sterling
        JPY,    // Japanese Yen
        CNY,    // Chinese Yuan
        AUD,    // Australian Dollar
        CAD,    // Canadian Dollar
        CHF,    // Swiss Franc
        INR,    // Indian Rupee
        RUB,    // Russian Ruble
        ZAR,    // South African Rand
        MXN,    // Mexican Peso
        ARS,    // Argentine Peso
        CLP,    // Chilean Peso
        COP,    // Colombian Peso
        KRW,    // South Korean Won
        SEK,    // Swedish Krona
        NOK,    // Norwegian Krone
        DKK,    // Danish Krone
        NZD,    // New Zealand Dollar
        SGD,    // Singapore Dollar
        HKD,    // Hong Kong Dollar
        TRY,    // Turkish Lira
        PLN,    // Polish Zloty
        CZK,    // Czech Koruna
        HUF,    // Hungarian Forint
        ILS,    // Israeli New Shekel
        AED,    // United Arab Emirates Dirham
        SAR,    // Saudi Riyal
        THB,    // Thai Baht
        MYR,    // Malaysian Ringgit
        IDR,    // Indonesian Rupiah
        PHP,    // Philippine Peso
        VND,    // Vietnamese Dong
        EGP,    // Egyptian Pound
        PKR,    // Pakistani Rupee
        TWD,    // New Taiwan Dollar
        KWD,    // Kuwaiti Dinar
        QAR,    // Qatari Riyal
        BHD,    // Bahraini Dinar
        OMR,    // Omani Rial
        MAD,    // Moroccan Dirham
        NGN,    // Nigerian Naira
        UAH,    // Ukrainian Hryvnia
        RON,    // Romanian Leu
        BGN,    // Bulgarian Lev
        HRK,    // Croatian Kuna
        LKR,    // Sri Lankan Rupee
        DZD,    // Algerian Dinar
        PEN,    // Peruvian Sol
        UYU,    // Uruguayan Peso
        PYG,    // Paraguayan Guarani
        BOB,    // Bolivian Boliviano
        GHS,    // Ghanaian Cedi
        KES,    // Kenyan Shilling
        TZS,    // Tanzanian Shilling
        UGX,    // Ugandan Shilling
        XOF,    // West African CFA franc
        XAF,    // Central African CFA franc
        XCD,    // East Caribbean Dollar
        JMD,    // Jamaican Dollar
        DOP,    // Dominican Peso
        GTQ,    // Guatemalan Quetzal
        HNL,    // Honduran Lempira
        NIO,    // Nicaraguan Córdoba
        CRC,    // Costa Rican Colón
        SVC,    // Salvadoran Colón
        BZD,    // Belize Dollar
        BBD,    // Barbadian Dollar
        BSD,    // Bahamian Dollar
        TTD,    // Trinidad and Tobago Dollar
        FJD,    // Fijian Dollar
        XPF,    // CFP Franc
        MUR,    // Mauritian Rupee
        SCR,    // Seychellois Rupee
        MVR,    // Maldivian Rufiyaa
        NPR,    // Nepalese Rupee
        BDT,    // Bangladeshi Taka
        MMK,    // Myanmar Kyat
        LAK,    // Lao Kip
        KHR,    // Cambodian Riel
        MNT,    // Mongolian Tögrög
        LBP,    // Lebanese Pound
        JOD,    // Jordanian Dinar
        SYP,    // Syrian Pound
        IQD,    // Iraqi Dinar
        IRR,    // Iranian Rial
        AFN,    // Afghan Afghani
        AZN,    // Azerbaijani Manat
        GEL,    // Georgian Lari
        AMD,    // Armenian Dram
        BYN,    // Belarusian Ruble
        KZT,    // Kazakhstani Tenge
        UZS,    // Uzbekistani Soʻm
        TMT,    // Turkmenistan Manat
        MZN,    // Mozambican Metical
        ZMW,    // Zambian Kwacha
        BWP,    // Botswana Pula
        SZL,    // Swazi Lilangeni
        NAD,    // Namibian Dollar
        LSL,    // Lesotho Loti
        MWK,    // Malawian Kwacha
        GMD,    // Gambian Dalasi
        SLL,    // Sierra Leonean Leone
        GNF,    // Central African CFA franc
        MGA,    // Malagasy Ariary
        KMF,    // Comorian Franc
        DJF,    // Djiboutian Franc
        SOS,    // Somali Shilling
        SDG,    // Sudanese Pound
        LYD,    // Libyan Dinar
        TND,    // Tunisian Dinar
        MRU,    // Mauritanian Ouguiya
        CVE,    // Cape Verdean Escudo
        STN,    // São Tomé and Príncipe Dobra
        ANG,    // Netherlands Antillean Guilder
        AWG,    // Aruban Florin
        SRD,    // Surinamese Dollar
        GYD,    // Guyanese Dollar
        KYD,    // Cayman Islands Dollar
        BMD,    // Bermudian Dollar
        FKP,    // Falkland Islands Pound
        SHP,    // Saint Helena Pound
        WST,    // Samoan Tala
        TOP,    // Tongan Paʻanga
        VUV,    // Vanuatu Vatu
        PGK,    // Papua New Guinean Kina
        SBD,    // Solomon Islands Dollar
        BTN,    // Bhutanese Ngultrum
        MOP,    // Macanese Pataca
        KPW,    // North Korean Won
        MRO,    // Mauritanian Ouguiya (old)
        SSP,    // South Sudanese Pound
        ZWL     // Zimbabwean Dollar            
    }

    public static class CurrencyExtensions
    {
        public static string ToSymbol(this Currency currency)
        {
            return currency switch
            {
                Currency.USD => "$",
                Currency.EUR => "€",
                Currency.BRL => "R$",
                Currency.GBP => "£",
                Currency.JPY => "¥",
                Currency.CNY => "¥",
                Currency.AUD => "A$",
                Currency.CAD => "C$",
                Currency.CHF => "CHF",
                Currency.INR => "₹",
                // Add more symbols as needed
                _ => currency.ToString() // Fallback to the enum name if no symbol is defined
            };
        }
    }
}

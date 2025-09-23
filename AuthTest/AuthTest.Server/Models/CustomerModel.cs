
using System.Xml.Serialization;
namespace AuthTest.Server.Models
{

    [XmlRoot(ElementName = "ФИООтв", Namespace = "")]
    public class ФИООтв
    {

        [XmlAttribute(AttributeName = "Фамилия", Namespace = "")]
        public string Фамилия { get; set; }

        [XmlAttribute(AttributeName = "Имя", Namespace = "")]
        public string Имя { get; set; }

        [XmlAttribute(AttributeName = "Отчество", Namespace = "")]
        public string Отчество { get; set; }
    }

    [XmlRoot(ElementName = "ИдОтпр", Namespace = "")]
    public class ИдОтпр
    {

        [XmlElement(ElementName = "ФИООтв", Namespace = "")]
        public ФИООтв ФИООтв { get; set; }

        [XmlAttribute(AttributeName = "ДолжОтв", Namespace = "")]
        public string ДолжОтв { get; set; }

        [XmlAttribute(AttributeName = "Тлф", Namespace = "")]
        public string Тлф { get; set; }
    }

    [XmlRoot(ElementName = "ГРНДата", Namespace = "")]
    public class ГРНДата
    {

        [XmlAttribute(AttributeName = "ГРН", Namespace = "")]
        public string ГРН { get; set; }

        [XmlAttribute(AttributeName = "ДатаЗаписи", Namespace = "")]
        public DateTime ДатаЗаписи { get; set; }
    }

    [XmlRoot(ElementName = "СвНаимЮЛ", Namespace = "")]
    public class СвНаимЮЛ
    {

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "НаимЮЛПолн", Namespace = "")]
        public string НаимЮЛПолн { get; set; }

        [XmlElement(ElementName = "СвНаимЮЛСокр", Namespace = "")]
        public СвНаимЮЛСокр СвНаимЮЛСокр { get; set; }
    }

    [XmlRoot(ElementName = "СвНаимЮЛСокр", Namespace = "")]
    public class СвНаимЮЛСокр
    {
        [XmlAttribute(AttributeName = "НаимСокр", Namespace = "")]
        public string? НаимСокр { get; set; }
    }

    [XmlRoot(ElementName = "АдрМНРФ", Namespace = "")]
    public class АдрМНРФ
    {
        [XmlElement(ElementName = "Регион", Namespace = "")]
        public Регион Регион { get; set; }

        [XmlElement(ElementName = "Город", Namespace = "")]
        public Город Город { get; set; }

        [XmlElement(ElementName = "Улица", Namespace = "")]
        public Улица Улица { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "Дом", Namespace = "")]
        public string Дом { get; set; }

        [XmlAttribute(AttributeName = "КодАдрКладр", Namespace = "")]
        public string КодАдрКладр { get; set; }

        [XmlAttribute(AttributeName = "КодРегион", Namespace = "")]
        public string КодРегион { get; set; }

        [XmlAttribute(AttributeName = "Индекс", Namespace = "")]
        public string Индекс { get; set; }
    }

    [XmlRoot(ElementName = "СвПодразд", Namespace = "")]
    public class СвПодразд
    {
        [XmlElement(ElementName = "СвФилиал", Namespace = "")]
        public List<СвФилиал> СвФилиал { get; set; }

    }

    [XmlRoot(ElementName = "СвФилиал", Namespace = "")]
    public class СвФилиал
    {
        [XmlElement(ElementName = "ГРНДатаПерв", Namespace = "")]
        public ГРНДатаПерв ГРНДатаПерв { get; set; }

        [XmlElement(ElementName = "СвНаим", Namespace = "")]
        public СвНаим СвНаим { get; set; }

        [XmlElement(ElementName = "АдрМНРФ", Namespace = "")]
        public АдрМНРФ АдрМНРФ { get; set; }
    }

    [XmlRoot(ElementName = "СвНаим", Namespace = "")]
    public class СвНаим
    {
        [XmlAttribute(AttributeName = "НаимПолн", Namespace = "")]
        public string НаимПолн { get; set; }
    }

    [XmlRoot(ElementName = "Регион", Namespace = "")]
    public class Регион
    {

        [XmlAttribute(AttributeName = "ТипРегион", Namespace = "")]
        public string ТипРегион { get; set; }

        [XmlAttribute(AttributeName = "НаимРегион", Namespace = "")]
        public string НаимРегион { get; set; }
    }

    [XmlRoot(ElementName = "Район", Namespace = "")]
    public class Район
    {

        [XmlAttribute(AttributeName = "ТипРайон", Namespace = "")]
        public string ТипРайон { get; set; }

        [XmlAttribute(AttributeName = "НаимРайон", Namespace = "")]
        public string НаимРайон { get; set; }
    }

    [XmlRoot(ElementName = "НаселПункт", Namespace = "")]
    public class НаселПункт
    {

        [XmlAttribute(AttributeName = "ТипНаселПункт", Namespace = "")]
        public string ТипНаселПункт { get; set; }

        [XmlAttribute(AttributeName = "НаимНаселПункт", Namespace = "")]
        public string НаимНаселПункт { get; set; }
    }

    [XmlRoot(ElementName = "Улица", Namespace = "")]
    public class Улица
    {

        [XmlAttribute(AttributeName = "ТипУлица", Namespace = "")]
        public string ТипУлица { get; set; }

        [XmlAttribute(AttributeName = "НаимУлица", Namespace = "")]
        public string НаимУлица { get; set; }
    }

    [XmlRoot(ElementName = "АдресРФ", Namespace = "")]
    public class АдресРФ
    {

        [XmlElement(ElementName = "Регион", Namespace = "")]
        public Регион Регион { get; set; }

        [XmlElement(ElementName = "Район", Namespace = "")]
        public Район Район { get; set; }

        [XmlElement(ElementName = "НаселПункт", Namespace = "")]
        public НаселПункт НаселПункт { get; set; }

        [XmlElement(ElementName = "Улица", Namespace = "")]
        public Улица Улица { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "Индекс", Namespace = "")]
        public string Индекс { get; set; }

        [XmlAttribute(AttributeName = "КодРегион", Namespace = "")]
        public string КодРегион { get; set; }

        [XmlAttribute(AttributeName = "КодАдрКладр", Namespace = "")]
        public string КодАдрКладр { get; set; }

        [XmlAttribute(AttributeName = "Дом", Namespace = "")]
        public string Дом { get; set; }

        [XmlAttribute(AttributeName = "Кварт", Namespace = "")]
        public string Кварт { get; set; }

        [XmlElement(ElementName = "Город", Namespace = "")]
        public Город Город { get; set; }

        [XmlAttribute(AttributeName = "ТекстРешИзмМН", Namespace = "")]
        public string ТекстРешИзмМН { get; set; }

        [XmlAttribute(AttributeName = "ТекстНедАдресЮЛ", Namespace = "")]
        public string ТекстНедАдресЮЛ { get; set; }
    }

    [XmlRoot(ElementName = "СвАдресЮЛ", Namespace = "")]
    public class СвАдресЮЛ
    {

        [XmlElement(ElementName = "АдресРФ", Namespace = "")]
        public АдресРФ АдресРФ { get; set; }

        [XmlElement(ElementName = "СвНедАдресЮЛ", Namespace = "")]
        public СвНедАдресЮЛ СвНедАдресЮЛ { get; set; }
    }

    [XmlRoot(ElementName = "СвНедАдресЮЛ", Namespace = "")]
    public class СвНедАдресЮЛ
    {

        [XmlAttribute(AttributeName = "ТекстНедАдресЮЛ", Namespace = "")]
        public string? ТекстНедАдресЮЛ { get; set; }

        [XmlAttribute(AttributeName = "СвНедАдресЮЛ", Namespace = "")]
        public string? ПризнНедАдресЮЛ { get; set; }
    }

    [XmlRoot(ElementName = "СвНедДанУчр", Namespace = "")]
    public class СвНедДанУчр
    {
        [XmlAttribute(AttributeName = "ТекстНедДанУчр", Namespace = "")]
        public string? ТекстНедДанУчр { get; set; }

        [XmlAttribute(AttributeName = "ПризнНедДанУчр", Namespace = "")]
        public string? ПризнНедДанУчр { get; set; }
    }

    [XmlRoot(ElementName = "СпОбрЮЛ", Namespace = "")]
    public class СпОбрЮЛ
    {

        [XmlAttribute(AttributeName = "КодСпОбрЮЛ", Namespace = "")]
        public string КодСпОбрЮЛ { get; set; }

        [XmlAttribute(AttributeName = "НаимСпОбрЮЛ", Namespace = "")]
        public string НаимСпОбрЮЛ { get; set; }
    }

    [XmlRoot(ElementName = "СвОбрЮЛ", Namespace = "")]
    public class СвОбрЮЛ
    {

        [XmlElement(ElementName = "СпОбрЮЛ", Namespace = "")]
        public СпОбрЮЛ СпОбрЮЛ { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "ОГРН", Namespace = "")]
        public string ОГРН { get; set; }

        [XmlAttribute(AttributeName = "ДатаОГРН", Namespace = "")]
        public DateTime ДатаОГРН { get; set; }

        [XmlAttribute(AttributeName = "РегНом", Namespace = "")]
        public string РегНом { get; set; }

        [XmlAttribute(AttributeName = "ДатаРег", Namespace = "")]
        public DateTime ДатаРег { get; set; }

        [XmlAttribute(AttributeName = "НаимРО", Namespace = "")]
        public string НаимРО { get; set; }
    }

    [XmlRoot(ElementName = "СвРегОрг", Namespace = "")]
    public class СвРегОрг
    {
        [XmlElement(ElementName = "ГРНИПДата")]
        public ГРНИПДата ГРНИПДата { get; set; }

        [XmlAttribute(AttributeName = "КодНО")]
        public string КодНО { get; set; }

        [XmlAttribute(AttributeName = "НаимНО")]
        public string НаимНО { get; set; }

        [XmlAttribute(AttributeName = "АдрРО")]
        public string АдрРО { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }
    }

    [XmlRoot(ElementName = "СвНО", Namespace = "")]
    public class СвНО
    {

        [XmlAttribute(AttributeName = "КодНО", Namespace = "")]
        public string КодНО { get; set; }

        [XmlAttribute(AttributeName = "НаимНО", Namespace = "")]
        public string НаимНО { get; set; }
    }

    [XmlRoot(ElementName = "СвУчетНО", Namespace = "")]
    public class СвУчетНО
    {

        [XmlElement(ElementName = "СвНО", Namespace = "")]
        public СвНО СвНО { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlElement(ElementName = "ГРНИПДата", Namespace = "")]
        public ГРНИПДата ГРНИПДата { get; set; }

        [XmlAttribute(AttributeName = "ИНН", Namespace = "")]
        public string ИНН { get; set; }

        [XmlAttribute(AttributeName = "ИННФЛ", Namespace = "")]
        public string ИННФЛ { get; set; }

        [XmlAttribute(AttributeName = "КПП", Namespace = "")]
        public string КПП { get; set; }

        [XmlAttribute(AttributeName = "ДатаПостУч", Namespace = "")]
        public DateTime ДатаПостУч { get; set; }

        [XmlElement(ElementName = "ГРНДатаИспр", Namespace = "")]
        public ГРНДатаИспр ГРНДатаИспр { get; set; }
    }

    [XmlRoot(ElementName = "СвОргПФ", Namespace = "")]
    public class СвОргПФ
    {

        [XmlAttribute(AttributeName = "КодПФ", Namespace = "")]
        public string КодПФ { get; set; }

        [XmlAttribute(AttributeName = "НаимПФ", Namespace = "")]
        public string НаимПФ { get; set; }
    }

    [XmlRoot(ElementName = "СвРегПФ", Namespace = "")]
    public class СвРегПФ
    {

        [XmlElement(ElementName = "СвОргПФ", Namespace = "")]
        public СвОргПФ СвОргПФ { get; set; }

        [XmlElement(ElementName = "ГРНИПДата", Namespace = "")]
        public ГРНИПДата ГРНИПДата { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "РегНомПФ", Namespace = "")]
        public string РегНомПФ { get; set; }

        [XmlAttribute(AttributeName = "ДатаРег", Namespace = "")]
        public DateTime ДатаРег { get; set; }
    }

    [XmlRoot(ElementName = "СвОргФСС", Namespace = "")]
    public class СвОргФСС
    {

        [XmlAttribute(AttributeName = "КодФСС", Namespace = "")]
        public string КодФСС { get; set; }

        [XmlAttribute(AttributeName = "НаимФСС", Namespace = "")]
        public string НаимФСС { get; set; }
    }

    [XmlRoot(ElementName = "СвРегФСС", Namespace = "")]
    public class СвРегФСС
    {

        [XmlElement(ElementName = "СвОргФСС", Namespace = "")]
        public СвОргФСС СвОргФСС { get; set; }

        [XmlElement(ElementName = "ГРНИПДата")]
        public ГРНИПДата ГРНИПДата { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "РегНомФСС", Namespace = "")]
        public string РегНомФСС { get; set; }

        [XmlAttribute(AttributeName = "ДатаРег", Namespace = "")]
        public DateTime ДатаРег { get; set; }
    }

    [XmlRoot(ElementName = "СвУстКап", Namespace = "")]
    public class СвУстКап
    {

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "НаимВидКап", Namespace = "")]
        public string НаимВидКап { get; set; }

        [XmlAttribute(AttributeName = "СумКап", Namespace = "")]
        public string СумКап { get; set; }
    }

    [XmlRoot(ElementName = "ГРНДатаПерв", Namespace = "")]
    public class ГРНДатаПерв
    {

        [XmlAttribute(AttributeName = "ГРН", Namespace = "")]
        public string ГРН { get; set; }

        [XmlAttribute(AttributeName = "ДатаЗаписи", Namespace = "")]
        public DateTime ДатаЗаписи { get; set; }
    }

    [XmlRoot(ElementName = "ФИОРус", Namespace = "")]
    public class ФИОРус
    {
        [XmlAttribute(AttributeName = "Фамилия", Namespace = "")]
        public string Фамилия { get; set; }

        [XmlAttribute(AttributeName = "Имя", Namespace = "")]
        public string Имя { get; set; }

        [XmlAttribute(AttributeName = "Отчество", Namespace = "")]
        public string Отчество { get; set; }
    }

    [XmlRoot(ElementName = "СвФЛ", Namespace = "")]
    public class СвФЛ
    {

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "Фамилия", Namespace = "")]
        public string Фамилия { get; set; }

        [XmlAttribute(AttributeName = "Имя", Namespace = "")]
        public string Имя { get; set; }

        [XmlAttribute(AttributeName = "Отчество", Namespace = "")]
        public string Отчество { get; set; }

        [XmlAttribute(AttributeName = "ИННФЛ", Namespace = "")]
        public string ИННФЛ { get; set; }

        [XmlElement(ElementName = "ГРНИПДата", Namespace = "")]
        public ГРНИПДата ГРНИПДата { get; set; }

        [XmlAttribute(AttributeName = "Пол", Namespace = "")]
        public string Пол { get; set; }

        [XmlElement(ElementName = "ФИОРус", Namespace = "")]
        public ФИОРус ФИОРус { get; set; }
    }

    [XmlRoot(ElementName = "СвДолжн", Namespace = "")]
    public class СвДолжн
    {

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "ВидДолжн", Namespace = "")]
        public string ВидДолжн { get; set; }

        [XmlAttribute(AttributeName = "НаимВидДолжн", Namespace = "")]
        public string НаимВидДолжн { get; set; }

        [XmlAttribute(AttributeName = "НаимДолжн", Namespace = "")]
        public string НаимДолжн { get; set; }
    }
    [XmlRoot(ElementName = "СвПолФЛ", Namespace = "")]
    public class СвПолФЛ
    {
        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "Пол", Namespace = "")]
        public string Пол { get; set; }
    }

    [XmlRoot(ElementName = "СвГраждФЛ", Namespace = "")]
    public class СвГраждФЛ
    {
        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "КодГражд", Namespace = "")]
        string КодГражд { get; set; }
    }

    [XmlRoot(ElementName = "СведДолжнФЛ", Namespace = "")]
    public class СведДолжнФЛ
    {

        [XmlElement(ElementName = "ГРНДатаПерв", Namespace = "")]
        public ГРНДатаПерв ГРНДатаПерв { get; set; }

        [XmlElement(ElementName = "СвФЛ", Namespace = "")]
        public СвФЛ СвФЛ { get; set; }

        [XmlElement(ElementName = "СвДолжн", Namespace = "")]
        public СвДолжн СвДолжн { get; set; }

        [XmlElement(ElementName = "СвПолФЛ", Namespace = "")]
        public СвПолФЛ СвПолФЛ { get; set; }

        [XmlElement(ElementName = "СвГраждФЛ", Namespace = "")]
        public СвГраждФЛ СвГраждФЛ { get; set; }
    }
    //В таблице СвДоляООО не хватает столбцов

    [XmlRoot(ElementName = "РазмерДоли", Namespace = "")]
    public class РазмерДоли
    {

        [XmlElement(ElementName = "Процент", Namespace = "")]
        public string Процент { get; set; }
    }

    [XmlRoot(ElementName = "ДоляУстКап", Namespace = "")]
    public class ДоляУстКап
    {

        [XmlElement(ElementName = "РазмерДоли", Namespace = "")]
        public РазмерДоли РазмерДоли { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "НоминСтоим", Namespace = "")]
        public string НоминСтоим { get; set; }


    }

    [XmlRoot(ElementName = "УчрФЛ", Namespace = "")]
    public class УчрФЛ
    {

        [XmlElement(ElementName = "ГРНДатаПерв", Namespace = "")]
        public ГРНДатаПерв ГРНДатаПерв { get; set; }

        [XmlElement(ElementName = "СвФЛ", Namespace = "")]
        public СвФЛ СвФЛ { get; set; }

        [XmlElement(ElementName = "ДоляУстКап", Namespace = "")]
        public ДоляУстКап ДоляУстКап { get; set; }

        [XmlElement(ElementName = "СвПолФЛ", Namespace = "")]
        public СвПолФЛ СвПолФЛ { get; set; }

        [XmlElement(ElementName = "СвГраждФЛ", Namespace = "")]
        public СвГраждФЛ СвГраждФЛ { get; set; }
    }


    [XmlRoot(ElementName = "СвУчредит", Namespace = "")]
    public class СвУчредит
    {
        [XmlElement(ElementName = "УчрФЛ", Namespace = "")]
        public List<УчрФЛ> УчрФЛ { get; set; }

        [XmlElement(ElementName = "УчрРФСубМО", Namespace = "")]
        public УчрРФСубМО УчрРФСубМО { get; set; }

        [XmlElement(ElementName = "УчрЮЛРос", Namespace = "")]
        public List<УчрЮЛРос> УчрЮЛРос { get; set; }
    }
    [XmlRoot(ElementName = "ГРНИПДата")]
    public class ГРНИПДата
    {

        [XmlAttribute(AttributeName = "ГРНИП")]
        public string ГРНИП { get; set; }

        [XmlAttribute(AttributeName = "ДатаЗаписи")]
        public DateTime ДатаЗаписи { get; set; }
    }

    [XmlRoot(ElementName = "СвОКВЭДОсн", Namespace = "")]
    public class СвОКВЭДОсн
    {
        [XmlElement(ElementName = "ГРНИПДата")]
        public ГРНИПДата ГРНИПДата { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "КодОКВЭД", Namespace = "")]
        public string КодОКВЭД { get; set; }

        [XmlAttribute(AttributeName = "НаимОКВЭД", Namespace = "")]
        public string НаимОКВЭД { get; set; }

        [XmlAttribute(AttributeName = "ПрВерсОКВЭД", Namespace = "")]
        public string ПрВерсОКВЭД { get; set; }
    }

    [XmlRoot(ElementName = "СвОКВЭДДоп", Namespace = "")]
    public class СвОКВЭДДоп
    {
        [XmlElement(ElementName = "ГРНИПДата")]
        public ГРНИПДата ГРНИПДата { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "КодОКВЭД", Namespace = "")]
        public string КодОКВЭД { get; set; }

        [XmlAttribute(AttributeName = "НаимОКВЭД", Namespace = "")]
        public string НаимОКВЭД { get; set; }

        [XmlAttribute(AttributeName = "ПрВерсОКВЭД", Namespace = "")]
        public string ПрВерсОКВЭД { get; set; }

        [XmlElement(ElementName = "ГРНДатаИспр", Namespace = "")]
        public ГРНДатаИспр ГРНДатаИспр { get; set; }
    }

    [XmlRoot(ElementName = "СвОКВЭД", Namespace = "")]
    public class СвОКВЭД
    {

        [XmlElement(ElementName = "СвОКВЭДОсн", Namespace = "")]
        public СвОКВЭДОсн СвОКВЭДОсн { get; set; }

        [XmlElement(ElementName = "СвОКВЭДДоп", Namespace = "")]
        public List<СвОКВЭДДоп> СвОКВЭДДоп { get; set; }
    }

    [XmlRoot(ElementName = "ВидЗап", Namespace = "")]
    public class ВидЗап
    {

        [XmlAttribute(AttributeName = "КодСПВЗ", Namespace = "")]
        public string КодСПВЗ { get; set; }

        [XmlAttribute(AttributeName = "НаимВидЗап", Namespace = "")]
        public string НаимВидЗап { get; set; }
    }

    [XmlRoot(ElementName = "СведПредДок", Namespace = "")]
    public class СведПредДок
    {

        [XmlElement(ElementName = "НаимДок", Namespace = "")]
        public string НаимДок { get; set; }

        [XmlElement(ElementName = "ДатаДок", Namespace = "")]
        public string ДатаДок { get; set; }

        [XmlElement(ElementName = "НомДок", Namespace = "")]
        public string НомДок { get; set; }
    }

    [XmlRoot(ElementName = "СвРеоргЮЛ", Namespace = "")]
    public class СвРеоргЮЛ
    {
        [XmlAttribute(AttributeName = "ОГРН", Namespace = "")]
        public string ОГРН { get; set; }

        [XmlAttribute(AttributeName = "ИНН", Namespace = "")]
        public string ИНН { get; set; }

        [XmlAttribute(AttributeName = "НаимЮЛПолн", Namespace = "")]
        public string НаимЮЛПолн { get; set; }

        [XmlAttribute(AttributeName = "СостЮЛпосле", Namespace = "")]
        public string СостЮЛпосле { get; set; }
    }

    [XmlRoot(ElementName = "СвРеорг", Namespace = "")]
    public class СвРеорг
    {
        [XmlElement(ElementName = "СвСтатус", Namespace = "")]
        public СвСтатус СвСтатус { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlElement(ElementName = "СвРеоргЮЛ", Namespace = "")]
        public СвРеоргЮЛ СвРеоргЮЛ { get; set; }
    }

    [XmlRoot(ElementName = "СвЗапЕГРЮЛ", Namespace = "")]
    public class СвЗапЕГРЮЛ
    {

        [XmlElement(ElementName = "ВидЗап", Namespace = "")]
        public ВидЗап ВидЗап { get; set; }

        [XmlElement(ElementName = "СвРегОрг", Namespace = "")]
        public СвРегОрг СвРегОрг { get; set; }

        [XmlElement(ElementName = "СведПредДок", Namespace = "")]
        public List<СведПредДок> СведПредДок { get; set; }

        [XmlAttribute(AttributeName = "ИдЗап", Namespace = "")]
        public string ИдЗап { get; set; }

        [XmlAttribute(AttributeName = "ГРН", Namespace = "")]
        public string ГРН { get; set; }

        [XmlAttribute(AttributeName = "ДатаЗап", Namespace = "")]
        public DateTime ДатаЗап { get; set; }

        [XmlText]
        public string text { get; set; }

        [XmlElement(ElementName = "СвСвид", Namespace = "")]
        public СвСвид СвСвид { get; set; }

        [XmlElement(ElementName = "СвСтатусЗап", Namespace = "")]
        public СвСтатусЗап СвСтатусЗап { get; set; }

        [XmlElement(ElementName = "ГРНДатаИспрПред", Namespace = "")]
        public ГРНДатаИспрПред ГРНДатаИспрПред { get; set; }
    }

    [XmlRoot(ElementName = "СвДоляООО")]
    public class СвДоляООО
    {
        [XmlElement(ElementName = "РазмерДоли", Namespace = "")]
        public РазмерДоли РазмерДоли { get; set; }

        [XmlAttribute(AttributeName = "НоминСтоим", Namespace = "")]
        public string НоминСтоим { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }
    }

    [XmlRoot(ElementName = "СвЮЛ", Namespace = "")]
    public class СвЮЛ
    {
        [XmlElement(ElementName = "СвСтатусКорневой", Namespace = "")]
        public СвСтатусКорневой СвСтатусКорневой { get; set; }

        [XmlElement(ElementName = "СвРеорг", Namespace = "")]
        public СвРеорг СвРеорг { get; set; }

        [XmlElement(ElementName = "СвПреем", Namespace = "")]
        public СвПреем СвПреем { get; set; }

        [XmlElement(ElementName = "СвПрекрЮЛ", Namespace = "")]
        public СвПрекрЮЛ СвПрекрЮЛ { get; set; }

        [XmlElement(ElementName = "СвНаимЮЛ", Namespace = "")]
        public СвНаимЮЛ СвНаимЮЛ { get; set; }

        [XmlElement(ElementName = "АдрМНРФ", Namespace = "")]
        public АдрМНРФ АдрМНРФ { get; set; }

        [XmlElement(ElementName = "СвАдресЮЛ", Namespace = "")]
        public СвАдресЮЛ СвАдресЮЛ { get; set; }

        [XmlElement(ElementName = "СвОбрЮЛ", Namespace = "")]
        public СвОбрЮЛ СвОбрЮЛ { get; set; }

        [XmlElement(ElementName = "СвРегОрг", Namespace = "")]
        public СвРегОрг СвРегОрг { get; set; }

        [XmlElement(ElementName = "СвУчетНО", Namespace = "")]
        public СвУчетНО СвУчетНО { get; set; }

        [XmlElement(ElementName = "СвРегПФ", Namespace = "")]
        public СвРегПФ СвРегПФ { get; set; }

        [XmlElement(ElementName = "СвРегФСС", Namespace = "")]
        public СвРегФСС СвРегФСС { get; set; }

        [XmlElement(ElementName = "СвУстКап", Namespace = "")]
        public СвУстКап СвУстКап { get; set; }

        [XmlElement(ElementName = "СведДолжнФЛ", Namespace = "")]
        public СведДолжнФЛ СведДолжнФЛ { get; set; }

        [XmlElement(ElementName = "СвУчредит", Namespace = "")]
        public СвУчредит СвУчредит { get; set; }

        [XmlElement(ElementName = "СвОКВЭД", Namespace = "")]
        public СвОКВЭД СвОКВЭД { get; set; }

        [XmlElement(ElementName = "СвЗапЕГРЮЛ", Namespace = "")]
        public List<СвЗапЕГРЮЛ> СвЗапЕГРЮЛ { get; set; }

        [XmlAttribute(AttributeName = "ДатаВып", Namespace = "")]
        public DateTime ДатаВып { get; set; }

        [XmlAttribute(AttributeName = "ОГРН", Namespace = "")]
        public string ОГРН { get; set; }

        [XmlAttribute(AttributeName = "ДатаОГРН", Namespace = "")]
        public DateTime ДатаОГРН { get; set; }

        [XmlAttribute(AttributeName = "ИНН", Namespace = "")]
        public string ИНН { get; set; }

        [XmlAttribute(AttributeName = "КПП", Namespace = "")]
        public string КПП { get; set; }

        [XmlAttribute(AttributeName = "СпрОПФ", Namespace = "")]
        public string СпрОПФ { get; set; }

        [XmlAttribute(AttributeName = "КодОПФ", Namespace = "")]
        public string КодОПФ { get; set; }

        [XmlAttribute(AttributeName = "ПолнНаимОПФ", Namespace = "")]
        public string ПолнНаимОПФ { get; set; }

        [XmlElement(ElementName = "СвДержРеестрАО", Namespace = "")]
        public СвДержРеестрАО СвДержРеестрАО { get; set; }

        [XmlElement(ElementName = "СвЛицензия", Namespace = "")]
        public List<СвЛицензия> СвЛицензия { get; set; }

        [XmlElement(ElementName = "СвДоляООО", Namespace = "")]
        public СвДоляООО СвДоляООО { get; set; }

        [XmlElement(ElementName = "СвПредш", Namespace = "")]
        public СвПредш СвПредш { get; set; }

        [XmlElement(ElementName = "СвПрекращ", Namespace = "")]
        public СвПрекращ СвПрекращ { get; set; }

        [XmlElement(ElementName = "СвПодразд", Namespace = "")]
        public СвПодразд СвПодразд { get; set; }
    }

    [XmlRoot(ElementName = "СвПреем")]
    public class СвПреем
    {
        [XmlElement(ElementName = "ГРНДата")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "ОГРН")]
        public string ОГРН { get; set; }

        [XmlAttribute(AttributeName = "НаимЮЛПолн")]
        public string НаимЮЛПолн { get; set; }

        [XmlAttribute(AttributeName = "ИНН")]
        public string ИНН { get; set; }
    }

    [XmlRoot(ElementName = "СвГражд")]
    public class СвГражд
    {

        [XmlElement(ElementName = "ГРНИПДата")]
        public ГРНИПДата ГРНИПДата { get; set; }

        [XmlAttribute(AttributeName = "ВидГражд")]
        public string ВидГражд { get; set; }
        [XmlAttribute(AttributeName = "ОКСМ")]
        public string ОКСМ { get; set; }
        [XmlAttribute(AttributeName = "НаимСтран")]
        public string НаимСтран { get; set; }
    }
    [XmlRoot(ElementName = "СвАдрМЖ")]
    public class СвАдрМЖ
    {

        [XmlElement(ElementName = "АдресРФ")]
        public АдресРФ АдресРФ { get; set; }

        [XmlElement(ElementName = "ГРНИПДата")]
        public ГРНИПДата ГРНИПДата { get; set; }
    }

    [XmlRoot(ElementName = "СвРегИП")]
    public class СвРегИП
    {

        [XmlAttribute(AttributeName = "ОГРНИП")]
        public string ОГРНИП { get; set; }

        [XmlAttribute(AttributeName = "ДатаОГРНИП")]
        public DateTime ДатаОГРНИП { get; set; }

        [XmlAttribute(AttributeName = "РегНом")]
        public string РегНом { get; set; }

        [XmlAttribute(AttributeName = "ДатаРег")]
        public DateTime ДатаРег { get; set; }

        [XmlAttribute(AttributeName = "НаимРО")]
        public string НаимРО { get; set; }

        [XmlAttribute(AttributeName = "ОГРН")]
        public string ОГРН { get; set; }

        [XmlAttribute(AttributeName = "ИНН")]
        public string ИНН { get; set; }

        [XmlAttribute(AttributeName = "НаимЮЛПолн")]
        public string НаимЮЛПолн { get; set; }

        [XmlAttribute(AttributeName = "ГРНИП")]
        public string ГРНИП { get; set; }

        [XmlAttribute(AttributeName = "ДатаЗаписи")]
        public DateTime ДатаЗаписи { get; set; }
    }
    [XmlRoot(ElementName = "СвЗапЕГРИП")]
    public class СвЗапЕГРИП
    {

        [XmlElement(ElementName = "ВидЗап")]
        public ВидЗап ВидЗап { get; set; }

        [XmlElement(ElementName = "СвРегОрг")]
        public СвРегОрг СвРегОрг { get; set; }

        [XmlElement(ElementName = "СведПредДок")]
        public List<СведПредДок> СведПредДок { get; set; }

        [XmlElement(ElementName = "СвСвид")]
        public СвСвид СвСвид { get; set; }

        [XmlAttribute(AttributeName = "ИдЗап")]
        public string ИдЗап { get; set; }

        [XmlAttribute(AttributeName = "ГРНИП")]
        public string ГРНИП { get; set; }

        [XmlAttribute(AttributeName = "ДатаЗап")]
        public DateTime ДатаЗап { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
    [XmlRoot(ElementName = "СвИП")]
    public class СвИП
    {

        [XmlElement(ElementName = "СвФЛ")]
        public СвФЛ СвФЛ { get; set; }

        [XmlElement(ElementName = "СвГражд")]
        public СвГражд СвГражд { get; set; }

        [XmlElement(ElementName = "СвАдрМЖ")]
        public СвАдрМЖ СвАдрМЖ { get; set; }

        [XmlElement(ElementName = "СвРегИП")]
        public СвРегИП СвРегИП { get; set; }

        [XmlElement(ElementName = "СвРегОрг")]
        public СвРегОрг СвРегОрг { get; set; }

        [XmlElement(ElementName = "СвУчетНО")]
        public СвУчетНО СвУчетНО { get; set; }

        [XmlElement(ElementName = "СвРегПФ")]
        public СвРегПФ СвРегПФ { get; set; }

        [XmlElement(ElementName = "СвРегФСС")]
        public СвРегФСС СвРегФСС { get; set; }

        [XmlElement(ElementName = "СвОКВЭД")]
        public СвОКВЭД СвОКВЭД { get; set; }

        [XmlElement(ElementName = "СвПрекращ", Namespace = "")]
        public СвПрекращ СвПрекращ { get; set; }

        [XmlElement(ElementName = "СвЛицензия", Namespace = "")]
        public List<СвЛицензия> СвЛицензия { get; set; }
        //Нет в БД (ЗапЕГРИП)

        [XmlElement(ElementName = "СвЗапЕГРИП")]
        public List<СвЗапЕГРИП> СвЗапЕГРИП { get; set; }


        [XmlAttribute(AttributeName = "ДатаВып")]
        public DateTime ДатаВып { get; set; }

        [XmlAttribute(AttributeName = "ОГРНИП")]
        public string ОГРНИП { get; set; }

        [XmlAttribute(AttributeName = "ДатаОГРНИП")]
        public DateTime ДатаОГРНИП { get; set; }

        [XmlAttribute(AttributeName = "ИННФЛ")]
        public string ИННФЛ { get; set; }

        [XmlAttribute(AttributeName = "КодВидИП")]
        public string КодВидИП { get; set; }

        [XmlAttribute(AttributeName = "НаимВидИП")]
        public string НаимВидИП { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "СвРешИсклЮЛ", Namespace = "")]
    public class СвРешИсклЮЛ
    {
        [XmlAttribute(AttributeName = "ДатаРеш")]
        public DateTime ДатаРеш { get; set; }

        [XmlAttribute(AttributeName = "НомерРеш")]
        public string НомерРеш { get; set; }

        [XmlAttribute(AttributeName = "ДатаПубликации")]
        public DateTime ДатаПубликации { get; set; }

        [XmlAttribute(AttributeName = "НомерЖурнала")]
        public string НомерЖурнала { get; set; }
    }

    [XmlRoot(ElementName = "СвСтатус", Namespace = "")]
    //Названия классов повторяются
    public class СвСтатусКорневой
    {
        [XmlElement(ElementName = "СвЗапЕГРИП")]
        public СвСтатус СвСтатус { get; set; }

        [XmlElement(ElementName = "ГРНДата")]
        public ГРНДата ГРНДата { get; set; }

        [XmlElement(ElementName = "СвРешИсклЮЛ")]
        public СвРешИсклЮЛ СвРешИсклЮЛ { get; set; }
    }

    [XmlRoot(ElementName = "СвСтатус", Namespace = "")]
    public class СвСтатус
    {
        [XmlAttribute(AttributeName = "КодСтатус", Namespace = "")]
        public string КодСтатус { get; set; }

        [XmlAttribute(AttributeName = "НаимСтатус", Namespace = "")]
        public string НаимСтатус { get; set; }

        [XmlAttribute(AttributeName = "ДатаПрекращ", Namespace = "")]
        public DateTime ДатаПрекращ { get; set; }
    }
    [XmlRoot(ElementName = "СвПредш", Namespace = "")]
    public class СвПредш
    {
        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "ОГРН", Namespace = "")]
        public string ОГРН { get; set; }

        [XmlAttribute(AttributeName = "ИНН", Namespace = "")]
        public string ИНН { get; set; }

        [XmlAttribute(AttributeName = "НаимЮЛПолн", Namespace = "")]
        public string НаимЮЛПолн { get; set; }
    }

    [XmlRoot(ElementName = "СпПрекрЮЛ", Namespace = "")]
    public class СпПрекрЮЛ
    {
        [XmlAttribute(AttributeName = "ДатаПрекрЮЛ", Namespace = "")]
        public DateTime ДатаПрекрЮЛ { get; set; }
    }

    [XmlRoot(ElementName = "СвПрекрЮЛ", Namespace = "")]
    public class СвПрекрЮЛ
    {
        [XmlElement(ElementName = "СпПрекрЮЛ", Namespace = "")]
        public СпПрекрЮЛ СпПрекрЮЛ { get; set; }

        [XmlElement(ElementName = "СвРегОрг", Namespace = "")]
        public СвРегОрг СвРегОрг { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }
    }

    [XmlRoot(ElementName = "СвПрекращ", Namespace = "")]
    public class СвПрекращ
    {
        [XmlElement(ElementName = "СвСтатус", Namespace = "")]
        public СвСтатус СвСтатус { get; set; }

        [XmlElement(ElementName = "ГРНИПДата", Namespace = "")]
        public ГРНИПДата ГРНИПДата { get; set; }
    }

    [XmlRoot(ElementName = "Документ", Namespace = "")]
    public class Документ
    {

        [XmlElement(ElementName = "СвЮЛ", Namespace = "")]
        public СвЮЛ СвЮЛ { get; set; }

        [XmlAttribute(AttributeName = "ИдДок", Namespace = "")]
        public string ИдДок { get; set; }

        [XmlText]
        public string text { get; set; }
        [XmlElement(ElementName = "СвИП")]
        public СвИП СвИП { get; set; }

        public bool retry { get; set; }
    }

    [XmlRoot(ElementName = "Город", Namespace = "")]
    public class Город
    {

        [XmlAttribute(AttributeName = "ТипГород", Namespace = "")]
        public string ТипГород { get; set; }

        [XmlAttribute(AttributeName = "НаимГород", Namespace = "")]
        public string НаимГород { get; set; }
    }

    [XmlRoot(ElementName = "СвСвид", Namespace = "")]
    public class СвСвид
    {

        [XmlAttribute(AttributeName = "Серия", Namespace = "")]
        public string Серия { get; set; }

        [XmlAttribute(AttributeName = "Номер", Namespace = "")]
        public string Номер { get; set; }

        [XmlAttribute(AttributeName = "ДатаВыдСвид", Namespace = "")]
        public DateTime ДатаВыдСвид { get; set; }
    }

    [XmlRoot(ElementName = "ГРНДатаИспр", Namespace = "")]
    public class ГРНДатаИспр
    {

        [XmlAttribute(AttributeName = "ГРН", Namespace = "")]
        public string ГРН { get; set; }

        [XmlAttribute(AttributeName = "ДатаЗаписи", Namespace = "")]
        public DateTime ДатаЗаписи { get; set; }

        [XmlAttribute(AttributeName = "ИдЗап", Namespace = "")]
        public string ИдЗап { get; set; }

        [XmlAttribute(AttributeName = "ДатаЗап", Namespace = "")]
        public DateTime ДатаЗап { get; set; }
    }

    [XmlRoot(ElementName = "НаимИННЮЛ", Namespace = "")]
    public class НаимИННЮЛ
    {

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "ИНН", Namespace = "")]
        public string ИНН { get; set; }

        [XmlAttribute(AttributeName = "НаимЮЛПолн", Namespace = "")]
        public string НаимЮЛПолн { get; set; }

        [XmlAttribute(AttributeName = "ОГРН", Namespace = "")]
        public string ОГРН { get; set; }
    }

    [XmlRoot(ElementName = "УчрЮЛРос", Namespace = "")]
    public class УчрЮЛРос
    {

        [XmlElement(ElementName = "ГРНДатаПерв", Namespace = "")]
        public ГРНДатаПерв ГРНДатаПерв { get; set; }

        [XmlElement(ElementName = "НаимИННЮЛ", Namespace = "")]
        public НаимИННЮЛ НаимИННЮЛ { get; set; }

        [XmlElement(ElementName = "ДоляУстКап", Namespace = "")]
        public ДоляУстКап ДоляУстКап { get; set; }

        [XmlElement(ElementName = "СвНедДанУчр", Namespace = "")]
        public СвНедДанУчр СвНедДанУчр { get; set; }
    }

    [XmlRoot(ElementName = "ДержРеестрАО", Namespace = "")]
    public class ДержРеестрАО
    {

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "ОГРН", Namespace = "")]
        public string ОГРН { get; set; }

        [XmlAttribute(AttributeName = "ИНН", Namespace = "")]
        public string ИНН { get; set; }

        [XmlAttribute(AttributeName = "НаимЮЛПолн", Namespace = "")]
        public string НаимЮЛПолн { get; set; }
    }

    [XmlRoot(ElementName = "СвДержРеестрАО", Namespace = "")]
    public class СвДержРеестрАО
    {

        [XmlElement(ElementName = "ДержРеестрАО", Namespace = "")]
        public ДержРеестрАО ДержРеестрАО { get; set; }
    }
    [XmlRoot(ElementName = "НаимЛицВидДеят", Namespace = "")]
    public class НаимЛицВидДеят
    {
    }

    [XmlRoot(ElementName = "СвЛицензия", Namespace = "")]
    public class СвЛицензия
    {

        [XmlElement(ElementName = "НаимЛицВидДеят", Namespace = "")]
        public string НаимЛицВидДеят { get; set; }

        [XmlElement(ElementName = "МестоДействЛиц", Namespace = "")]
        public string МестоДействЛиц { get; set; }

        [XmlElement(ElementName = "ЛицОргВыдЛиц", Namespace = "")]
        public string ЛицОргВыдЛиц { get; set; }

        [XmlElement(ElementName = "ГРНИПДата")]
        public ГРНИПДата ГРНИПДата { get; set; }

        [XmlElement(ElementName = "ГРНДата", Namespace = "")]
        public ГРНДата ГРНДата { get; set; }

        [XmlAttribute(AttributeName = "НомЛиц", Namespace = "")]
        public string НомЛиц { get; set; }

        [XmlAttribute(AttributeName = "ДатаЛиц", Namespace = "")]
        public DateTime ДатаЛиц { get; set; }

        [XmlAttribute(AttributeName = "ДатаНачЛиц", Namespace = "")]
        public DateTime ДатаНачЛиц { get; set; }

        [XmlAttribute(AttributeName = "ДатаОкончЛиц", Namespace = "")]
        public DateTime ДатаОкончЛиц { get; set; }

        [XmlText]
        public string text { get; set; }
    }

    [XmlRoot(ElementName = "СвСтатусЗап", Namespace = "")]
    public class СвСтатусЗап
    {

        [XmlElement(ElementName = "ГРНДатаИспр", Namespace = "")]
        public ГРНДатаИспр ГРНДатаИспр { get; set; }
    }

    [XmlRoot(ElementName = "ГРНДатаИспрПред", Namespace = "")]
    public class ГРНДатаИспрПред
    {

        [XmlAttribute(AttributeName = "ИдЗап", Namespace = "")]
        public string ИдЗап { get; set; }

        [XmlAttribute(AttributeName = "ГРН", Namespace = "")]
        public string ГРН { get; set; }

        [XmlAttribute(AttributeName = "ДатаЗап", Namespace = "")]
        public DateTime ДатаЗап { get; set; }
    }

    [XmlRoot(ElementName = "ВидНаимУчр", Namespace = "")]
    public class ВидНаимУчр
    {

        [XmlAttribute(AttributeName = "КодУчрРФСубМО", Namespace = "")]
        public string КодУчрРФСубМО { get; set; }

        [XmlAttribute(AttributeName = "КодРегион", Namespace = "")]
        public string КодРегион { get; set; }

        [XmlAttribute(AttributeName = "НаимРегион", Namespace = "")]
        public string НаимРегион { get; set; }
    }

    [XmlRoot(ElementName = "СвОргОсущПр", Namespace = "")]
    public class СвОргОсущПр
    {

        [XmlElement(ElementName = "ГРНДатаПерв", Namespace = "")]
        public ГРНДатаПерв ГРНДатаПерв { get; set; }

        [XmlElement(ElementName = "НаимИННЮЛ", Namespace = "")]
        public НаимИННЮЛ НаимИННЮЛ { get; set; }
    }

    [XmlRoot(ElementName = "УчрРФСубМО", Namespace = "")]
    public class УчрРФСубМО
    {

        [XmlElement(ElementName = "ГРНДатаПерв", Namespace = "")]
        public ГРНДатаПерв ГРНДатаПерв { get; set; }

        [XmlElement(ElementName = "ВидНаимУчр", Namespace = "")]
        public ВидНаимУчр ВидНаимУчр { get; set; }

        [XmlElement(ElementName = "СвОргОсущПр", Namespace = "")]
        public СвОргОсущПр СвОргОсущПр { get; set; }
    }

    [XmlRoot(ElementName = "Файл", Namespace = "")]
    public class Файл
    {

        [XmlElement(ElementName = "ИдОтпр", Namespace = "")]
        public ИдОтпр ИдОтпр { get; set; }

        [XmlElement(ElementName = "Документ", Namespace = "")]
        public List<Документ> Документ { get; set; }

        [XmlAttribute(AttributeName = "ИдФайл", Namespace = "")]
        public string ИдФайл { get; set; }

        [XmlAttribute(AttributeName = "ВерсФорм", Namespace = "")]
        public string ВерсФорм { get; set; }

        [XmlAttribute(AttributeName = "ТипИнф", Namespace = "")]
        public string ТипИнф { get; set; }

        [XmlAttribute(AttributeName = "ВерсПрог", Namespace = "")]
        public string ВерсПрог { get; set; }

        [XmlAttribute(AttributeName = "КолДок", Namespace = "")]
        public string КолДок { get; set; }

        [XmlText]
        public string text { get; set; }
    }






}

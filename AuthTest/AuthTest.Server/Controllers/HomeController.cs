using AuthTest.Server.Models;
using EgrWebEntity.ModelTable;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;

namespace AuthTest.Server.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    //[AllowAnonymous]
    public class HomeController : Controller
    {
        //private DbContextTable _dbcontext;
        private readonly IDbContextFactory<DbContextTable> _contextFactory;
        public int docCount = 0;
        public int finishedWorkersCount = 0;
        public object locker = new();
        public List<object> listLockers = new List<object> { new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), };
        public static List<string> entitiesInWork = new List<string>();
        public HomeController(IDbContextFactory<DbContextTable> contextFactory)
        {
            _contextFactory = contextFactory;
            int nWorkers; // number of processing threads
            int nCompletions; // number of I/O threads
            ThreadPool.GetMaxThreads(out nWorkers, out nCompletions);
            nWorkers = 70;
            ThreadPool.SetMaxThreads(nWorkers, nCompletions);
        }

        public IActionResult Index()
        {
            using (DbContextTable _dbcontext = _contextFactory.CreateDbContext())
            {
                TableViewModel tableViewModel = new TableViewModel();
                tableViewModel.ЕГРИП_СвФЛ = _dbcontext.ЕГРИП_СвФЛ?.ToList();
                tableViewModel.ЕГРЮЛ_СвЮЛ = _dbcontext.ЕГРЮЛ_СвЮЛ.ToList();
                tableViewModel.ЕГРИП_ОКВЭД = _dbcontext.ЕГРИП_ОКВЭД.ToList();
                tableViewModel.ЕГРИП_СвАдрМЖ = _dbcontext.ЕГРИП_СвАдрМЖ.ToList();
                tableViewModel.ЕГРИП_СвГражд = _dbcontext.ЕГРИП_СвГражд.ToList();
                tableViewModel.ЕГРИП_СвИП = _dbcontext.ЕГРИП_СвИП.ToList();
                //tableViewModel.ЕГРИП_СвЛицензия = _dbcontext.ЕГРИП_СвЛицензия.ToList();
                tableViewModel.ЕГРИП_СвПрекращ = _dbcontext.ЕГРИП_СвПрекращ.ToList();
                tableViewModel.ЕГРИП_СвРегИП = _dbcontext.ЕГРИП_СвРегИП.ToList();
                tableViewModel.ЕГРИП_СвРегОрг = _dbcontext.ЕГРИП_СвРегОрг?.ToList();
                tableViewModel.ЕГРИП_СвРегПФ = _dbcontext.ЕГРИП_СвРегПФ.ToList();
                tableViewModel.ЕГРИП_СвРегФСС = _dbcontext.ЕГРИП_СвРегФСС.ToList();
                tableViewModel.ЕГРИП_СвУчетНО = _dbcontext.ЕГРИП_СвУчетНО.ToList();
                tableViewModel.ЕГРЮЛ_ОКВЭД = _dbcontext.ЕГРЮЛ_ОКВЭД.ToList();
                //tableViewModel.ЕГРЮЛ_СвАдресЮЛ = _dbcontext.ЕГРЮЛ_СвАдресЮЛ.ToList();
                tableViewModel.ЕГРЮЛ_СвДержРеестрАО = _dbcontext.ЕГРЮЛ_СвДержРеестрАО?.ToList();
                tableViewModel.ЕГРЮЛ_СвДоляООО = _dbcontext.ЕГРЮЛ_СвДоляООО?.ToList();
                tableViewModel.ЕГРЮЛ_СвЗапЕГРЮЛ = _dbcontext.ЕГРЮЛ_СвЗапЕГРЮЛ.ToList();
                tableViewModel.ЕГРЮЛ_СвЛицензия = _dbcontext.ЕГРЮЛ_СвЛицензия.ToList();
                tableViewModel.ЕГРЮЛ_СвНаимЮЛ = _dbcontext.ЕГРЮЛ_СвНаимЮЛ.ToList();
                //tableViewModel.ЕГРЮЛ_СвОбрЮЛ = _dbcontext.ЕГРЮЛ_СвОбрЮЛ.ToList();
                tableViewModel.ЕГРЮЛ_СвПодразд = _dbcontext.ЕГРЮЛ_СвПодразд.ToList();
                tableViewModel.ЕГРЮЛ_СвПредш = _dbcontext.ЕГРЮЛ_СвПредш.ToList();
                tableViewModel.ЕГРЮЛ_СвПреем = _dbcontext.ЕГРЮЛ_СвПреем.ToList();
                tableViewModel.ЕГРЮЛ_СвПрекрЮЛ = _dbcontext.ЕГРЮЛ_СвПрекрЮЛ.ToList();
                tableViewModel.ЕГРЮЛ_СвРегОрг = _dbcontext.ЕГРЮЛ_СвРегОрг?.ToList();
                tableViewModel.ЕГРЮЛ_СвРегПФ = _dbcontext.ЕГРЮЛ_СвРегПФ.ToList();
                tableViewModel.ЕГРЮЛ_СвРегФСС = _dbcontext.ЕГРЮЛ_СвРегФСС.ToList();
                tableViewModel.ЕГРЮЛ_СвРеорг = _dbcontext.ЕГРЮЛ_СвРеорг.ToList();
                tableViewModel.ЕГРЮЛ_СвСтатус = _dbcontext.ЕГРЮЛ_СвСтатус?.ToList();
                tableViewModel.ЕГРЮЛ_СвУчетНО = _dbcontext.ЕГРЮЛ_СвУчетНО.ToList();
                tableViewModel.ЕГРЮЛ_СвУчредит = _dbcontext.ЕГРЮЛ_СвУчредит.ToList();
                tableViewModel.ЕГРЮЛ_СведДолжнФЛ = _dbcontext.ЕГРЮЛ_СведДолжнФЛ.ToList();
            }

            return View();
        }

        public IActionResult AuthPage()
        {
            return RedirectToPage("/Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [DisableRequestSizeLimit,
        RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> Загрузить(IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл пустой");
            }

            entitiesInWork.Clear();
            IPSubTables.ClearIPSubTables();
            ULSubTables.ClearULSubTables();
            finishedWorkersCount = 0;

            if (Path.GetExtension(file.FileName) == ".xml")
            {
                using var stream = new StreamReader(file.OpenReadStream(), Encoding.GetEncoding(1251));
                var serializer = new XmlSerializer(typeof(Файл));
                var model = serializer.Deserialize(stream) as Файл;

                docCount += Convert.ToInt32(model.КолДок);

                if (model.ТипИнф.Equals("ЕГРЮЛ_ОТКР_СВЕД"))
                {
                    foreach (Документ document in model.Документ)
                    {
                        try
                        {
                            ThreadPool.QueueUserWorkItem(ParseULDataDB, document);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                else
                {
                    foreach (Документ document in model.Документ)
                    {
                        try
                        {
                            ThreadPool.QueueUserWorkItem(ParseIPDataDB, document);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }

                while (docCount != finishedWorkersCount)
                {
                    Thread.Sleep(5000);
                }

                return View("LoadingDone");
            }
            else if (Path.GetExtension(file.FileName) == ".zip")
            {
                var fileName = Path.GetFileName(file.FileName);
                // Сохраняем файл на сервере
                var path = Path.Combine(Path.GetTempPath(), fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                using (var uploadedZip = ZipFile.Open(path, ZipArchiveMode.Read))
                {
                    // цикл по всем файлам в архиве
                    foreach (var entry in uploadedZip.Entries)
                    {
                        using (var entryStream = entry.Open())
                        {
                            var serializer = new XmlSerializer(typeof(Файл));
                            var model = serializer.Deserialize(entryStream) as Файл;

                            docCount += Convert.ToInt32(model.КолДок);

                            if (model.ТипИнф.Equals("ЕГРЮЛ_ОТКР_СВЕД"))
                            {
                                foreach (Документ document in model.Документ)
                                {
                                    try
                                    {
                                        ThreadPool.QueueUserWorkItem(ParseULDataDB, document);
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                }
                            }
                            else
                            {
                                foreach (Документ document in model.Документ)
                                {
                                    try
                                    {
                                        ThreadPool.QueueUserWorkItem(ParseIPDataDB, document);
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return BadRequest("Неподдерживаемый формат файла");
            }

            while (docCount != finishedWorkersCount)
            {
                Thread.Sleep(5000);
            }

            return View("LoadingDone");
        }

        [Authorize]
        [HttpGet("AllTab")]
        public IActionResult AllTab([FromQuery]  string buttonId)
        {
            TableViewModel tableViewModel = new TableViewModel();

            using (DbContextTable _dbcontext = _contextFactory.CreateDbContext())
            {
                switch (buttonId)
                {
                    case "Alltable":
                        tableViewModel.ЕГРИП_СвИП = _dbcontext.ЕГРИП_СвИП.ToList();
                        tableViewModel.ЕГРЮЛ_СвЮЛ = _dbcontext.ЕГРЮЛ_СвЮЛ.ToList();
                        return Json(tableViewModel);
                    case "IPtable":
                        tableViewModel.ЕГРИП_СвИП = _dbcontext.ЕГРИП_СвИП.ToList();
                        return Json(tableViewModel);
                    case "ULtable":
                        tableViewModel.ЕГРЮЛ_СвЮЛ = _dbcontext.ЕГРЮЛ_СвЮЛ.ToList();
                        return Json(tableViewModel);
                    default:
                        return BadRequest("Некорректный buttonId");
                }
            }
        }


        [HttpGet]
        public IActionResult Details(string table, int id)
        {
            List<dynamic> details = new List<dynamic>();

            using (DbContextTable _dbcontext = _contextFactory.CreateDbContext())
            {
                //получаем записи по таблице table
                var dbSet = _dbcontext.Set(Type.GetType("EgrWebEntity.ModelTable." + table + ", EgrWebEntity, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null"));
                foreach (dynamic entity in dbSet)
                {
                    //приводим к интерфейсу чтобы можно было обратиться к полю idЛицо
                    var entry = (IGenericTable)entity;

                    if (entry.idЛицо == id)
                    {
                        details.Add(entity);
                    }
                }
            }

            return Json(details);
        }

        [HttpGet]
        public IActionResult GetExtraTable(string table)
        {
            IQueryable? dbSet;
            using (DbContextTable _dbcontext = _contextFactory.CreateDbContext())
            {
                dbSet = _dbcontext.Set(Type.GetType("EgrWebEntity.ModelTable." + table + ", EgrWebEntity, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null"));
            }

            return Json(dbSet);
        }

        [HttpGet]
        public IActionResult GetLogs(string table, string INN)
        {
            List<ChangeLog> logs = new List<ChangeLog>();

            using (DbContextTable _dbcontext = _contextFactory.CreateDbContext())
            {
                logs.AddRange(from l in _dbcontext.ИсторияИзменений where l.Таблица == table && l.ИНН == INN select l);
            }

            return Json(logs);
        }

        [HttpGet]
        public IActionResult GetINNByEntityID(int id)
        {
            string inn;

            using (DbContextTable _dbcontext = _contextFactory.CreateDbContext())
            {
                inn = (from u in _dbcontext.ЮрЛицо where u.Id == id select u).FirstOrDefault().ИНН;
            }

            return Json(inn);
        }

        public async void ParseULDataDB(object documentObject)
        {
            Документ documentXML = (Документ)documentObject;
            bool firstTime = false;

            if (documentXML.СвЮЛ == null)
            {
                return;
            }

            // если с текущим ЮЛ работает другой процесс ждем пока закончится обработка
            //while (entitiesInWork.Contains(documentXML.СвЮЛ.ИНН))
            //{
            //    Console.WriteLine($"Лицо {documentXML.СвЮЛ.ИНН} уже в обработке, ожидание завершения.");
            //    Thread.Sleep(5000);
            //}
            // добавляем ЮЛ в список обрабатываемых
            // entitiesInWork.Add(documentXML.СвЮЛ.ИНН);

            using (DbContextTable _dbcontext = _contextFactory.CreateDbContext())
            {
                //создаем объект для работы с конкретным ЮЛ в бд
                UL ULDB = new UL();

                //ищем запись в бд
                UL data = (from u in _dbcontext.ЮрЛицо where u.ИНН == documentXML.СвЮЛ.ИНН select u).FirstOrDefault();
                if (data != null)
                {
                    ULDB = data;
                }
                else
                {
                    lock (locker)
                    {
                        data = ULSubTables.ULInProcessing.Where(e => e.ИНН == documentXML.СвЮЛ.ИНН).FirstOrDefault();
                    }

                    if (data != null)
                    {
                        ULDB = data;
                    }
                    else
                    {
                        //если записи нет заполняем поля согласно данным из xml
                        ULDB = new UL { ДатаОГРН = documentXML.СвЮЛ.ДатаОГРН, ОГРН = documentXML.СвЮЛ.ОГРН, ИНН = documentXML.СвЮЛ.ИНН, КПП = documentXML.СвЮЛ.КПП, СпрОПФ = documentXML.СвЮЛ.СпрОПФ, КодОПФ = documentXML.СвЮЛ.КодОПФ, ПолнНаимОПФ = documentXML.СвЮЛ.ПолнНаимОПФ, ИдДок = documentXML.ИдДок };
                        firstTime = true;
                        _dbcontext.ЮрЛицо.Add(ULDB);
                        //добавляем запись в бд (чтобы работал внешний ключ у подчиненных записей)
                        _dbcontext.SaveChanges();

                        lock (locker)
                        {
                            ULSubTables.ULInProcessing.Add(ULDB);
                        }
                    }

                }

                //добавляем запись о xml документе
                Document documentDB = new Document();
                documentDB.ДатаЗагрузки = DateTime.Now;
                documentDB.ИдДок = documentXML.ИдДок;
                documentDB.idЮЛ = ULDB.Id;

                ULDB.document = new List<Document> { documentDB };
                _dbcontext.ЮрЛицо.Update(ULDB);
                _dbcontext.SaveChanges();

                //проходим по всем таблицам дбконтекста
                foreach (var entity in _dbcontext.Model.GetEntityTypes())
                {
                    switch (entity.ShortName())
                    {
                        case "EGRULOKVED":
                            if (documentXML.СвЮЛ.СвОКВЭД == null)
                            {
                                continue;
                            }
                            else
                            {
                                List<EGRULOKVED> previousEntries;
                                var currentEntries = new List<EGRULOKVED>();
                                var currentEntry = new EGRULOKVED { Id = Guid.NewGuid().ToString(), Версия = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.ПрВерсОКВЭД, ГРНИП = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.ГРНДата.ГРН, ДатаГРНИП = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.ГРНДата.ДатаЗаписи, КодОквэд = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.КодОКВЭД, Наименование = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.НаимОКВЭД, ОснКод = true, idЛицо = ULDB.Id };
                                DateTime? currentEntriesDate = documentXML.СвЮЛ.СвОКВЭД.СвОКВЭДОсн != null ? documentXML.СвЮЛ.СвОКВЭД.СвОКВЭДОсн?.ГРНДата.ДатаЗаписи : documentXML.СвЮЛ.СвОКВЭД.СвОКВЭДДоп?.FirstOrDefault().ГРНДата.ДатаЗаписи;

                                lock (listLockers[0])
                                {
                                    previousEntries = ULSubTables.ЕГРЮЛ_ОКВЭД_Insert.Where(e => e.idЛицо == ULDB.Id).ToList();

                                    if (previousEntries.Count != 0)
                                    {
                                        if (previousEntries.FirstOrDefault()?.ДатаГРНИП <= currentEntriesDate)
                                        {
                                            ULSubTables.ЕГРЮЛ_ОКВЭД_Insert.RemoveAll(e => e.idЛицо == ULDB.Id);
                                            ULSubTables.ЕГРЮЛ_ОКВЭД_Insert.Add(currentEntry);

                                            foreach (СвОКВЭДДоп okvedDop in documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДДоп)
                                            {
                                                currentEntry = new EGRULOKVED { Id = Guid.NewGuid().ToString(), КодОквэд = okvedDop.КодОКВЭД, ОснКод = false, Наименование = okvedDop.НаимОКВЭД, Версия = okvedDop.ПрВерсОКВЭД, ГРНИП = okvedDop.ГРНДата.ГРН, ДатаГРНИП = okvedDop.ГРНДата.ДатаЗаписи, idЛицо = ULDB.Id };
                                                currentEntries.Add(currentEntry);
                                                ULSubTables.ЕГРЮЛ_ОКВЭД_Insert.Add(currentEntry);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (firstTime == false)
                                        {
                                            _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_ОКВЭД).Load();

                                            if (ULDB.ЕГРЮЛ_ОКВЭД?.FirstOrDefault() != null)
                                            {
                                                previousEntries = ULDB.ЕГРЮЛ_ОКВЭД.ToList();
                                                ULSubTables.ЕГРЮЛ_ОКВЭД_Delete.Add(ULDB.ЕГРЮЛ_ОКВЭД?.FirstOrDefault().idЛицо.ToString());
                                            }
                                        }

                                        ULSubTables.ЕГРЮЛ_ОКВЭД_Insert.Add(currentEntry);

                                        foreach (СвОКВЭДДоп okvedDop in documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДДоп)
                                        {
                                            currentEntry = new EGRULOKVED { Id = Guid.NewGuid().ToString(), КодОквэд = okvedDop.КодОКВЭД, ОснКод = false, Наименование = okvedDop.НаимОКВЭД, Версия = okvedDop.ПрВерсОКВЭД, ГРНИП = okvedDop.ГРНДата.ГРН, ДатаГРНИП = okvedDop.ГРНДата.ДатаЗаписи, idЛицо = ULDB.Id };
                                            currentEntries.Add(currentEntry);
                                            ULSubTables.ЕГРЮЛ_ОКВЭД_Insert.Add(currentEntry);
                                        }
                                    }
                                }


                                if (previousEntries?.Count > 0 && previousEntries.FirstOrDefault()?.ДатаГРНИП <= currentEntriesDate)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var oldEntry in previousEntries)
                                    {
                                        var newEntry = currentEntries.Where(e => e.КодОквэд == oldEntry.КодОквэд || e.Наименование == oldEntry.Наименование).FirstOrDefault();

                                        if (newEntry == null)
                                        {
                                            continue;
                                        }

                                        foreach (var property in typeof(EGRULOKVED).GetProperties())
                                        {
                                            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                            {
                                                if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                    continue;

                                                ChangeLog changes = new ChangeLog();

                                                changes.Таблица = "EGRULOKVED";
                                                changes.Столбец = property.Name;
                                                changes.ЗначениеДо = property.GetValue(oldEntry)?.ToString();
                                                changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                                changes.ИНН = documentXML.СвЮЛ.ИНН;
                                                changes.ДатаИзменения = DateTime.Now;

                                                //_dbcontext.ИсторияИзменений.Add(changes);
                                                changeList.Add(changes);
                                            }
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            ////подгружаем данные текущей таблицы для лица, которое сейчас обрабатываем
                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_ОКВЭД).Load();

                            //if (ULDB.ЕГРЮЛ_ОКВЭД?.Count > 0)
                            //{
                            //    foreach (EGRULOKVED entry in ULDB.ЕГРЮЛ_ОКВЭД)
                            //    {
                            //        //помечаем старые записи в бд на удаление
                            //        _dbcontext.Entry(entry).State = EntityState.Deleted;
                            //    }

                            //    //сохраняем старые записи для истории изменений
                            //    var previousEntries = ULDB.ЕГРЮЛ_ОКВЭД;
                            //    //записываем основной ОКВЭД
                            //    ULDB.ЕГРЮЛ_ОКВЭД = new List<EGRULOKVED> { new EGRULOKVED { Id = Guid.NewGuid().ToString(), Версия = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.ПрВерсОКВЭД, ГРНИП = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.ГРНДата.ГРН, ДатаГРНИП = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.ГРНДата.ДатаЗаписи, КодОквэд = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.КодОКВЭД, Наименование = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.НаимОКВЭД, ОснКод = true, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_ОКВЭД.FirstOrDefault()).State = EntityState.Added;

                            //    //записываем доп ОКВЭДы
                            //    foreach (СвОКВЭДДоп okvedDop in documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДДоп)
                            //    {
                            //        ULDB.ЕГРЮЛ_ОКВЭД.Add(new EGRULOKVED { Id = Guid.NewGuid().ToString(), КодОквэд = okvedDop.КодОКВЭД, ОснКод = false, Наименование = okvedDop.НаимОКВЭД, Версия = okvedDop.ПрВерсОКВЭД, ГРНИП = okvedDop.ГРНДата.ГРН, ДатаГРНИП = okvedDop.ГРНДата.ДатаЗаписи, idЛицо = ULDB.Id });
                            //        _dbcontext.Entry(ULDB.ЕГРЮЛ_ОКВЭД.Last()).State = EntityState.Added;
                            //    }

                            //    var currentEntries = ULDB.ЕГРЮЛ_ОКВЭД;

                            //    foreach (var oldEntry in previousEntries)
                            //    {
                            //        //сопоставляем старую запись с новой (если таковая имеется)
                            //        var newEntry = currentEntries.Where(e => e.КодОквэд == oldEntry.КодОквэд || e.Наименование == oldEntry.Наименование).FirstOrDefault();

                            //        if (newEntry == null)
                            //        {
                            //            continue;
                            //        }

                            //        //цикл по всем полям класса текущей таблицы
                            //        foreach (var property in typeof(EGRULOKVED).GetProperties())
                            //        {
                            //            //сравниваем одноименные поля записей
                            //            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                            //            {
                            //                if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                            //                    continue;

                            //                //записываем изменения
                            //                ChangeLog changes = new ChangeLog();

                            //                changes.Таблица = "EGRULOKVED";
                            //                changes.Столбец = property.Name;
                            //                changes.ЗначениеДо = property.GetValue(oldEntry)?.ToString();
                            //                changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                            //                changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //                changes.ДатаИзменения = DateTime.Now;

                            //                //_dbcontext.ИсторияИзменений.Add(changes);
                            //            }
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    //если по текущей таблице, у лица нет записей просто записываем все что есть
                            //    ULDB.ЕГРЮЛ_ОКВЭД = new List<EGRULOKVED> { new EGRULOKVED { Id = Guid.NewGuid().ToString(), Версия = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.ПрВерсОКВЭД, ГРНИП = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.ГРНДата.ГРН, ДатаГРНИП = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.ГРНДата.ДатаЗаписи, КодОквэд = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.КодОКВЭД, Наименование = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.НаимОКВЭД, ОснКод = true, idЛицо = ULDB.Id } };

                            //    foreach (СвОКВЭДДоп okvedDop in documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДДоп)
                            //    {
                            //        ULDB.ЕГРЮЛ_ОКВЭД.Add(new EGRULOKVED { Id = Guid.NewGuid().ToString(), КодОквэд = okvedDop.КодОКВЭД, ОснКод = false, Наименование = okvedDop.НаимОКВЭД, Версия = okvedDop.ПрВерсОКВЭД, ГРНИП = okvedDop.ГРНДата.ГРН, ДатаГРНИП = okvedDop.ГРНДата.ДатаЗаписи, idЛицо = ULDB.Id });
                            //    }
                            //}

                            break;

                        case "EGRULSvAddressUL":
                            if (documentXML.СвЮЛ.СвАдресЮЛ == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvAddressUL previousEntry;
                                var newEntry = new EGRULSvAddressUL { Id = Guid.NewGuid().ToString(), Индекс = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Индекс, Кварт = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Кварт, Дом = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Дом, КодАдрКладр = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.КодАдрКладр, КодРегион = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.КодРегион, НаимРегион = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Регион?.НаимРегион, НаимГород = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Город?.НаимГород, ТипГород = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Город?.ТипГород, НаимУлица = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Улица?.НаимУлица, ТипУлица = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Улица?.ТипУлица, ГРН = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.ГРНДата.ГРН, ДатаЗаписи = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.ГРНДата.ДатаЗаписи, НаимНаселПункт = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.НаселПункт?.НаимНаселПункт, ТипНаселПункт = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.НаселПункт?.ТипНаселПункт, ТипРегион = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Регион?.ТипРегион, НаимРайон = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Район?.НаимРайон, ТипРайон = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Район?.ТипРайон, ПризнНедАдресЮЛ = documentXML.СвЮЛ.СвАдресЮЛ.СвНедАдресЮЛ?.ПризнНедАдресЮЛ, ТекстНедАдресЮЛ = documentXML.СвЮЛ.СвАдресЮЛ.СвНедАдресЮЛ?.ТекстНедАдресЮЛ, idЛицо = ULDB.Id };

                                lock (listLockers[1])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвАдресЮЛ_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[1])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвАдресЮЛ_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвАдресЮЛ_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвАдресЮЛ).Load();

                                        if (ULDB.ЕГРЮЛ_СвАдресЮЛ?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвАдресЮЛ?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвАдресЮЛ_Delete.Add(ULDB.ЕГРЮЛ_СвАдресЮЛ?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[1])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвАдресЮЛ_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.ГРНДата.ДатаЗаписи)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRULSvAddressUL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new()
                                            {
                                                Таблица = "EGRULSvAddressUL",
                                                Столбец = property.Name,
                                                ЗначениеДо = property.GetValue(previousEntry)?.ToString(),
                                                ЗначениеПосле = property.GetValue(newEntry)?.ToString(),
                                                ИНН = documentXML.СвЮЛ.ИНН,
                                                ДатаИзменения = DateTime.Now
                                            };

                                            // //_dbcontext.ИсторияИзменений.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвАдресЮЛ).Load();

                            //if (ULDB.ЕГРЮЛ_СвАдресЮЛ.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвАдресЮЛ.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвАдресЮЛ.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвАдресЮЛ = new List<EGRULSvAddressUL> { new EGRULSvAddressUL { Id = ULDB.ЕГРЮЛ_СвАдресЮЛ.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвАдресЮЛ.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), Индекс = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Индекс, Кварт = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Кварт, Дом = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Дом, КодАдрКладр = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.КодАдрКладр, КодРегион = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.КодРегион, НаимРегион = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Регион?.НаимРегион, НаимГород = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Город?.НаимГород, ТипГород = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Город?.ТипГород, НаимУлица = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Улица?.НаимУлица, ТипУлица = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Улица?.ТипУлица, ГРН = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.ГРНДата.ГРН, ДатаЗаписи = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.ГРНДата.ДатаЗаписи, НаимНаселПункт = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.НаселПункт?.НаимНаселПункт, ТипНаселПункт = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.НаселПункт?.ТипНаселПункт, ТипРегион = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Регион?.ТипРегион, НаимРайон = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Район?.НаимРайон, ТипРайон = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Район?.ТипРайон, ПризнНедАдресЮЛ = documentXML.СвЮЛ.СвАдресЮЛ.СвНедАдресЮЛ?.ПризнНедАдресЮЛ, ТекстНедАдресЮЛ = documentXML.СвЮЛ.СвАдресЮЛ.СвНедАдресЮЛ?.ТекстНедАдресЮЛ, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвАдресЮЛ.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvAddressUL).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвАдресЮЛ.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvAddressUL";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвАдресЮЛ.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвАдресЮЛ = new List<EGRULSvAddressUL> { new EGRULSvAddressUL { Id = Guid.NewGuid().ToString(), Индекс = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Индекс, Кварт = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Кварт, Дом = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Дом, КодАдрКладр = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.КодАдрКладр, КодРегион = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.КодРегион, НаимРегион = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Регион?.НаимРегион, НаимГород = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Город?.НаимГород, ТипГород = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Город?.ТипГород, НаимУлица = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Улица?.НаимУлица, ТипУлица = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Улица?.ТипУлица, ГРН = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.ГРНДата.ГРН, ДатаЗаписи = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.ГРНДата.ДатаЗаписи, НаимНаселПункт = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.НаселПункт?.НаимНаселПункт, ТипНаселПункт = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.НаселПункт?.ТипНаселПункт, ТипРегион = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Регион?.ТипРегион, НаимРайон = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Район?.НаимРайон, ТипРайон = documentXML.СвЮЛ.СвАдресЮЛ.АдресРФ?.Район?.ТипРайон, ПризнНедАдресЮЛ = documentXML.СвЮЛ.СвАдресЮЛ.СвНедАдресЮЛ?.ПризнНедАдресЮЛ, ТекстНедАдресЮЛ = documentXML.СвЮЛ.СвАдресЮЛ.СвНедАдресЮЛ?.ТекстНедАдресЮЛ, idЛицо = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvDerjRegistryAO":
                            if (documentXML.СвЮЛ.СвДержРеестрАО == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvDerjRegistryAO previousEntry;
                                var newEntry = new EGRULSvDerjRegistryAO { Id = Guid.NewGuid().ToString(), ИНН = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.ИНН, ОГРН = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.ОГРН, НаимЮЛПолн = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.НаимЮЛПолн, ДатаЗаписи = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.ГРНДата.ГРН, idЛицо = ULDB.Id };

                                lock (listLockers[2])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвДержРеестрАО_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[2])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвДержРеестрАО.ДержРеестрАО.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвДержРеестрАО_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвДержРеестрАО_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвДержРеестрАО).Load();

                                        if (ULDB.ЕГРЮЛ_СвДержРеестрАО?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвДержРеестрАО?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвДержРеестрАО_Delete.Add(ULDB.ЕГРЮЛ_СвДержРеестрАО?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[2])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвДержРеестрАО_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвДержРеестрАО.ДержРеестрАО.ГРНДата.ДатаЗаписи)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRULSvDerjRegistryAO).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new()
                                            {
                                                Таблица = "EGRULSvDerjRegistryAO",
                                                Столбец = property.Name,
                                                ЗначениеДо = property.GetValue(previousEntry)?.ToString(),
                                                ЗначениеПосле = property.GetValue(newEntry)?.ToString(),
                                                ИНН = documentXML.СвЮЛ.ИНН,
                                                ДатаИзменения = DateTime.Now
                                            };

                                            // //_dbcontext.ИсторияИзменений.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвДержРеестрАО).Load();

                            //if (ULDB.ЕГРЮЛ_СвДержРеестрАО?.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвДержРеестрАО?.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвДержРеестрАО?.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвДержРеестрАО = new List<EGRULSvDerjRegistryAO> { new EGRULSvDerjRegistryAO { Id = ULDB.ЕГРЮЛ_СвДержРеестрАО?.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвДержРеестрАО?.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ИНН = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.ИНН, ОГРН = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.ОГРН, НаимЮЛПолн = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.НаимЮЛПолн, ДатаЗаписи = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвДержРеестрАО?.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvDerjRegistryAO).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвДержРеестрАО?.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvDerjRegistryAO";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвДержРеестрАО?.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвДержРеестрАО = new List<EGRULSvDerjRegistryAO> { new EGRULSvDerjRegistryAO { Id = Guid.NewGuid().ToString(), ИНН = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.ИНН, ОГРН = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.ОГРН, НаимЮЛПолн = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.НаимЮЛПолн, ДатаЗаписи = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвДержРеестрАО?.ДержРеестрАО?.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvShareOOO":
                            if (documentXML.СвЮЛ.СвДоляООО == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvShareOOO previousEntry;
                                (EGRULSvShareOOO Entry, DateTime? Date) EntryDatePair;
                                var newEntry = new EGRULSvShareOOO { Id = Guid.NewGuid().ToString(), НоминСтоим = documentXML.СвЮЛ.СвДоляООО?.НоминСтоим, idЛицо = ULDB.Id };

                                lock (listLockers[3])
                                {
                                    EntryDatePair = ULSubTables.ЕГРЮЛ_СвДоляООО_Insert.Where(e => e.Entry.idЛицо == ULDB.Id).FirstOrDefault();
                                    previousEntry = EntryDatePair.Entry;
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[3])
                                    {
                                        if (EntryDatePair.Date <= documentXML.СвЮЛ.СвДоляООО.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвДоляООО_Insert.RemoveAll(e => e.Entry.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвДоляООО_Insert.Add((newEntry, documentXML.СвЮЛ.СвДоляООО?.ГРНДата.ДатаЗаписи));
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвДоляООО).Load();

                                        if (ULDB.ЕГРЮЛ_СвДоляООО?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвДоляООО?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвДоляООО_Delete.Add(ULDB.ЕГРЮЛ_СвДоляООО?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[3])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвДоляООО_Insert.Add((newEntry, documentXML.СвЮЛ.СвДоляООО?.ГРНДата.ДатаЗаписи));
                                    }
                                }

                                if (previousEntry != null && EntryDatePair.Date <= documentXML.СвЮЛ.СвДоляООО.ГРНДата.ДатаЗаписи)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRULSvShareOOO).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new()
                                            {
                                                Таблица = "EGRULSvShareOOO",
                                                Столбец = property.Name,
                                                ЗначениеДо = property.GetValue(previousEntry)?.ToString(),
                                                ЗначениеПосле = property.GetValue(newEntry)?.ToString(),
                                                ИНН = documentXML.СвЮЛ.ИНН,
                                                ДатаИзменения = DateTime.Now
                                            };

                                            // //_dbcontext.ИсторияИзменений.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвДоляООО).Load();

                            //if (ULDB.ЕГРЮЛ_СвДоляООО?.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвДоляООО?.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвДоляООО?.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвДоляООО = new List<EGRULSvShareOOO> { new EGRULSvShareOOO { Id = ULDB.ЕГРЮЛ_СвДоляООО?.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвДоляООО?.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), НоминСтоим = documentXML.СвЮЛ.СвДоляООО?.НоминСтоим, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвДоляООО?.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvShareOOO).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвДоляООО?.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvShareOOO";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвДоляООО?.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвДоляООО = new List<EGRULSvShareOOO> { new EGRULSvShareOOO { Id = Guid.NewGuid().ToString(), НоминСтоим = documentXML.СвЮЛ.СвДоляООО?.НоминСтоим, idЛицо = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvZapEGRUL":
                            if (documentXML.СвЮЛ.СвЗапЕГРЮЛ == null || documentXML.СвЮЛ.СвЗапЕГРЮЛ.Count == 0)
                            {
                                continue;
                            }
                            else
                            {
                                List<EGRULSvZapEGRUL> previousEntries;
                                var currentEntries = new List<EGRULSvZapEGRUL>();
                                DateTime? currentEntriesDate = documentXML.СвЮЛ.СвЗапЕГРЮЛ.Select(d => d.ДатаЗап).ToArray()?.Max();

                                lock (listLockers[4])
                                {
                                    previousEntries = ULSubTables.ЕГРЮЛ_СвЗапЕГРЮЛ_Insert.Where(e => e.idЛицо == ULDB.Id).ToList();

                                    if (previousEntries.Count != 0)
                                    {
                                        if (previousEntries.Select(d => d.ДатаЗап).ToArray()?.Max() <= currentEntriesDate)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвЗапЕГРЮЛ_Insert.RemoveAll(e => e.idЛицо == previousEntries.FirstOrDefault().idЛицо);

                                            foreach (СвЗапЕГРЮЛ zapEGRUL in documentXML.СвЮЛ.СвЗапЕГРЮЛ)
                                            {
                                                foreach (СведПредДок svedPredDoc in zapEGRUL.СведПредДок)
                                                {
                                                    EGRULSvZapEGRUL zapEGRULBD = new EGRULSvZapEGRUL { Id = Guid.NewGuid().ToString(), ГРН = zapEGRUL.ГРН, ДатаЗап = zapEGRUL.ДатаЗап, ИдЗап = zapEGRUL.ИдЗап, НаимВидЗап = zapEGRUL.ВидЗап?.НаимВидЗап, КодСПВЗ = zapEGRUL.ВидЗап?.КодСПВЗ, НаимНО = zapEGRUL.СвРегОрг?.НаимНО, КодНО = zapEGRUL.СвРегОрг?.КодНО, idЛицо = ULDB.Id };
                                                    zapEGRULBD.НаимДок = svedPredDoc.НаимДок;
                                                    zapEGRULBD.НомДок = svedPredDoc.НомДок;
                                                    zapEGRULBD.ДатаДок = svedPredDoc.ДатаДок;

                                                    if (zapEGRUL.СвСвид != null)
                                                    {
                                                        zapEGRULBD.Серия = zapEGRUL.СвСвид.Серия;
                                                        zapEGRULBD.Номер = zapEGRUL.СвСвид.Номер;
                                                        zapEGRULBD.ДатаВыдСвид = zapEGRUL.СвСвид.ДатаВыдСвид;
                                                    }

                                                    ULSubTables.ЕГРЮЛ_СвЗапЕГРЮЛ_Insert.Add(zapEGRULBD);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (firstTime == false)
                                        {
                                            _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвЗапЕГРЮЛ).Load();

                                            if (ULDB.ЕГРЮЛ_СвЗапЕГРЮЛ?.FirstOrDefault() != null)
                                            {
                                                //previousEntries = ULDB.ЕГРЮЛ_СвЗапЕГРЮЛ.ToList();
                                                ULSubTables.ЕГРЮЛ_СвЗапЕГРЮЛ_Delete.Add(ULDB.ЕГРЮЛ_СвЗапЕГРЮЛ?.FirstOrDefault().idЛицо.ToString());
                                            }
                                        }

                                        foreach (СвЗапЕГРЮЛ zapEGRUL in documentXML.СвЮЛ.СвЗапЕГРЮЛ)
                                        {
                                            foreach (СведПредДок svedPredDoc in zapEGRUL.СведПредДок)
                                            {
                                                EGRULSvZapEGRUL zapEGRULBD = new EGRULSvZapEGRUL { Id = Guid.NewGuid().ToString(), ГРН = zapEGRUL.ГРН, ДатаЗап = zapEGRUL.ДатаЗап, ИдЗап = zapEGRUL.ИдЗап, НаимВидЗап = zapEGRUL.ВидЗап?.НаимВидЗап, КодСПВЗ = zapEGRUL.ВидЗап?.КодСПВЗ, НаимНО = zapEGRUL.СвРегОрг?.НаимНО, КодНО = zapEGRUL.СвРегОрг?.КодНО, idЛицо = ULDB.Id };
                                                zapEGRULBD.НаимДок = svedPredDoc.НаимДок;
                                                zapEGRULBD.НомДок = svedPredDoc.НомДок;
                                                zapEGRULBD.ДатаДок = svedPredDoc.ДатаДок;

                                                if (zapEGRUL.СвСвид != null)
                                                {
                                                    zapEGRULBD.Серия = zapEGRUL.СвСвид.Серия;
                                                    zapEGRULBD.Номер = zapEGRUL.СвСвид.Номер;
                                                    zapEGRULBD.ДатаВыдСвид = zapEGRUL.СвСвид.ДатаВыдСвид;
                                                }

                                                ULSubTables.ЕГРЮЛ_СвЗапЕГРЮЛ_Insert.Add(zapEGRULBD);
                                            }
                                        }
                                    }
                                }

                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвЗапЕГРЮЛ).Load();

                            //if (ULDB.ЕГРЮЛ_СвЗапЕГРЮЛ?.Count > 0)
                            //{
                            //    foreach (var entry in ULDB.ЕГРЮЛ_СвЗапЕГРЮЛ)
                            //    {
                            //        _dbcontext.Entry(entry).State = EntityState.Deleted;
                            //    }

                            //    //var previousEntries = ULDB.ЕГРЮЛ_СвЗапЕГРЮЛ;

                            //    foreach (СвЗапЕГРЮЛ zapEGRUL in documentXML.СвЮЛ.СвЗапЕГРЮЛ)
                            //    {
                            //        foreach (СведПредДок svedPredDoc in zapEGRUL.СведПредДок)
                            //        {
                            //            EGRULSvZapEGRUL zapEGRULBD = new EGRULSvZapEGRUL { Id = Guid.NewGuid().ToString(), ГРН = zapEGRUL.ГРН, ДатаЗап = zapEGRUL.ДатаЗап, ИдЗап = zapEGRUL.ИдЗап, НаимВидЗап = zapEGRUL.ВидЗап?.НаимВидЗап, КодСПВЗ = zapEGRUL.ВидЗап?.КодСПВЗ, НаимНО = zapEGRUL.СвРегОрг?.НаимНО, КодНО = zapEGRUL.СвРегОрг?.КодНО, idЛицо = ULDB.Id };
                            //            zapEGRULBD.НаимДок = svedPredDoc.НаимДок;
                            //            zapEGRULBD.НомДок = svedPredDoc.НомДок;
                            //            zapEGRULBD.ДатаДок = svedPredDoc.ДатаДок;

                            //            if (zapEGRUL.СвСвид != null)
                            //            {
                            //                zapEGRULBD.Серия = zapEGRUL.СвСвид.Серия;
                            //                zapEGRULBD.Номер = zapEGRUL.СвСвид.Номер;
                            //                zapEGRULBD.ДатаВыдСвид = zapEGRUL.СвСвид.ДатаВыдСвид;
                            //            }

                            //            ULDB.ЕГРЮЛ_СвЗапЕГРЮЛ.Add(zapEGRULBD);
                            //            _dbcontext.Entry(ULDB.ЕГРЮЛ_СвЗапЕГРЮЛ.Last()).State = EntityState.Added;
                            //        }
                            //    }

                            //    //var currentEntries = ULDB.ЕГРЮЛ_СвЗапЕГРЮЛ;

                            //    //foreach (var oldEntry in previousEntries)
                            //    //{
                            //    //    var newEntry = currentEntries.Where(e => e.КодОквэд == oldEntry.КодОквэд || e.Наименование == oldEntry.Наименование).FirstOrDefault();

                            //    //    if (newEntry == null)
                            //    //    {
                            //    //        continue;
                            //    //    }

                            //    //    foreach (var property in typeof(EGRULOKVED).GetProperties())
                            //    //    {
                            //    //        if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                            //    //        {
                            //    //            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ИП")
                            //    //                continue;

                            //    //            ChangeLog changes = new ChangeLog();

                            //    //            changes.Таблица = "EGRULOKVED";
                            //    //            changes.Столбец = property.Name;
                            //    //            changes.ЗначениеДо = property.GetValue(oldEntry)?.ToString();
                            //    //            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                            //    //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //    //            changes.ДатаИзменения = DateTime.Now;

                            //    //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //    //        }
                            //    //    }
                            //    //}
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвЗапЕГРЮЛ = new List<EGRULSvZapEGRUL>();

                            //    foreach (СвЗапЕГРЮЛ zapEGRUL in documentXML.СвЮЛ.СвЗапЕГРЮЛ)
                            //    {
                            //        foreach (СведПредДок svedPredDoc in zapEGRUL.СведПредДок)
                            //        {
                            //            EGRULSvZapEGRUL zapEGRULBD = new EGRULSvZapEGRUL { Id = Guid.NewGuid().ToString(), ГРН = zapEGRUL.ГРН, ДатаЗап = zapEGRUL.ДатаЗап, ИдЗап = zapEGRUL.ИдЗап, НаимВидЗап = zapEGRUL.ВидЗап?.НаимВидЗап, КодСПВЗ = zapEGRUL.ВидЗап?.КодСПВЗ, НаимНО = zapEGRUL.СвРегОрг?.НаимНО, КодНО = zapEGRUL.СвРегОрг?.КодНО, idЛицо = ULDB.Id };
                            //            zapEGRULBD.НаимДок = svedPredDoc.НаимДок;
                            //            zapEGRULBD.НомДок = svedPredDoc.НомДок;
                            //            zapEGRULBD.ДатаДок = svedPredDoc.ДатаДок;

                            //            if (zapEGRUL.СвСвид != null)
                            //            {
                            //                zapEGRULBD.Серия = zapEGRUL.СвСвид.Серия;
                            //                zapEGRULBD.Номер = zapEGRUL.СвСвид.Номер;
                            //                zapEGRULBD.ДатаВыдСвид = zapEGRUL.СвСвид.ДатаВыдСвид;
                            //            }

                            //            ULDB.ЕГРЮЛ_СвЗапЕГРЮЛ.Add(zapEGRULBD);
                            //        }
                            //    }
                            //}

                            break;

                        case "EGRULSvLicense":
                            if (documentXML.СвЮЛ.СвЛицензия == null || documentXML.СвЮЛ.СвЛицензия.Count == 0)
                            {
                                continue;
                            }
                            else
                            {
                                List<EGRULSvLicense> previousEntries;
                                var currentEntries = new List<EGRULSvLicense>();
                                var dateArray = documentXML.СвЮЛ.СвЛицензия.Where(x => x.ГРНДата != null).Select(d => d.ГРНДата.ДатаЗаписи).ToArray();
                                DateTime? currentEntriesDate = dateArray.Length != 0 ? dateArray.Max() : null;

                                lock (listLockers[5])
                                {
                                    previousEntries = ULSubTables.ЕГРЮЛ_СвЛицензия_Insert.Where(e => e.idЛицо == ULDB.Id).ToList();
                                }
                                if (previousEntries.Count != 0)
                                {
                                    lock (listLockers[5])
                                    {
                                        if (previousEntries.Select(d => d.ДатаЗаписи).ToArray()?.Max() <= currentEntriesDate)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвЛицензия_Insert.RemoveAll(e => e.idЛицо == ULDB.Id);

                                            foreach (var svLicense in documentXML.СвЮЛ.СвЛицензия)
                                            {
                                                ULSubTables.ЕГРЮЛ_СвЛицензия_Insert.Add(new EGRULSvLicense { Id = Guid.NewGuid().ToString(), ДатаНачЛиц = svLicense.ДатаНачЛиц, ДатаЛиц = svLicense.ДатаЛиц, НомЛиц = svLicense.НомЛиц, НаимЛицВидДеят = svLicense.НаимЛицВидДеят, ЛицОргВыдЛиц = svLicense.ЛицОргВыдЛиц, ДатаОкончЛиц = svLicense.ДатаОкончЛиц, ДатаЗаписи = svLicense.ГРНДата?.ДатаЗаписи, ГРН = svLicense.ГРНДата?.ГРН, idЛицо = ULDB.Id });
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвЛицензия).Load();

                                        if (ULDB.ЕГРЮЛ_СвЛицензия?.FirstOrDefault() != null)
                                        {
                                            previousEntries = ULDB.ЕГРЮЛ_СвЛицензия.ToList();
                                            ULSubTables.ЕГРЮЛ_СвЛицензия_Delete.Add(ULDB.ЕГРЮЛ_СвЛицензия?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[5])
                                    {
                                        foreach (var svLicense in documentXML.СвЮЛ.СвЛицензия)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвЛицензия_Insert.Add(new EGRULSvLicense { Id = Guid.NewGuid().ToString(), ДатаНачЛиц = svLicense.ДатаНачЛиц, ДатаЛиц = svLicense.ДатаЛиц, НомЛиц = svLicense.НомЛиц, НаимЛицВидДеят = svLicense.НаимЛицВидДеят, ЛицОргВыдЛиц = svLicense.ЛицОргВыдЛиц, ДатаОкончЛиц = svLicense.ДатаОкончЛиц, ДатаЗаписи = svLicense.ГРНДата?.ДатаЗаписи, ГРН = svLicense.ГРНДата?.ГРН, idЛицо = ULDB.Id });
                                        }
                                    }
                                }

                                if (previousEntries?.Count > 0 && previousEntries.Select(d => d.ДатаЗаписи).ToArray()?.Max() <= currentEntriesDate)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var oldEntry in previousEntries)
                                    {
                                        var newEntry = currentEntries.Where(e => e.НомЛиц == oldEntry.НомЛиц).FirstOrDefault();

                                        if (newEntry == null)
                                        {
                                            continue;
                                        }

                                        foreach (var property in typeof(EGRULSvLicense).GetProperties())
                                        {
                                            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                            {
                                                if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                    continue;

                                                ChangeLog changes = new ChangeLog();

                                                changes.Таблица = "EGRULSvLicense";
                                                changes.Столбец = property.Name;
                                                changes.ЗначениеДо = property.GetValue(oldEntry)?.ToString();
                                                changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                                changes.ИНН = documentXML.СвЮЛ.ИНН;
                                                changes.ДатаИзменения = DateTime.Now;

                                                changeList.Add(changes);
                                            }
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвЛицензия).Load();

                            //if (ULDB.ЕГРЮЛ_СвЛицензия?.Count > 0)
                            //{
                            //    foreach (var entry in ULDB.ЕГРЮЛ_СвЛицензия)
                            //    {
                            //        _dbcontext.Entry(entry).State = EntityState.Deleted;
                            //    }

                            //    var previousEntries = ULDB.ЕГРЮЛ_СвЛицензия;

                            //    ULDB.ЕГРЮЛ_СвЛицензия = new List<EGRULSvLicense>();

                            //    foreach (var svLicense in documentXML.СвЮЛ.СвЛицензия)
                            //    {
                            //        ULDB.ЕГРЮЛ_СвЛицензия.Add(new EGRULSvLicense { Id = Guid.NewGuid().ToString(), ДатаНачЛиц = svLicense.ДатаНачЛиц, ДатаЛиц = svLicense.ДатаЛиц, НомЛиц = svLicense.НомЛиц, НаимЛицВидДеят = svLicense.НаимЛицВидДеят, ЛицОргВыдЛиц = svLicense.ЛицОргВыдЛиц, ДатаОкончЛиц = svLicense.ДатаОкончЛиц, ДатаЗаписи = svLicense.ГРНДата.ДатаЗаписи, ГРН = svLicense.ГРНДата.ГРН, idЛицо = ULDB.Id });
                            //        _dbcontext.Entry(ULDB.ЕГРЮЛ_СвЛицензия.Last()).State = EntityState.Added;
                            //    }

                            //    var currentEntries = ULDB.ЕГРЮЛ_СвЛицензия;

                            //    foreach (var oldEntry in previousEntries)
                            //    {
                            //        var newEntry = currentEntries.Where(e => e.НомЛиц == oldEntry.НомЛиц).FirstOrDefault();

                            //        if (newEntry == null)
                            //        {
                            //            continue;
                            //        }

                            //        foreach (var property in typeof(EGRULSvLicense).GetProperties())
                            //        {
                            //            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                            //            {
                            //                if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                            //                    continue;

                            //                ChangeLog changes = new ChangeLog();

                            //                changes.Таблица = "EGRULSvLicense";
                            //                changes.Столбец = property.Name;
                            //                changes.ЗначениеДо = property.GetValue(oldEntry)?.ToString();
                            //                changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                            //                changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //                changes.ДатаИзменения = DateTime.Now;

                            //                //_dbcontext.ИсторияИзменений.Add(changes);
                            //            }
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвЛицензия = new List<EGRULSvLicense>();

                            //    foreach (var svLicense in documentXML.СвЮЛ.СвЛицензия)
                            //    {
                            //        ULDB.ЕГРЮЛ_СвЛицензия.Add(new EGRULSvLicense { Id = Guid.NewGuid().ToString(), ДатаНачЛиц = svLicense.ДатаНачЛиц, ДатаЛиц = svLicense.ДатаЛиц, НомЛиц = svLicense.НомЛиц, НаимЛицВидДеят = svLicense.НаимЛицВидДеят, ЛицОргВыдЛиц = svLicense.ЛицОргВыдЛиц, ДатаОкончЛиц = svLicense.ДатаОкончЛиц, ДатаЗаписи = svLicense.ГРНДата.ДатаЗаписи, ГРН = svLicense.ГРНДата.ГРН, idЛицо = ULDB.Id });
                            //    }
                            //}

                            break;

                        case "EGRULSvNaimUL":
                            if (documentXML.СвЮЛ.СвНаимЮЛ == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvNaimUL previousEntry;
                                var newEntry = new EGRULSvNaimUL { Id = Guid.NewGuid().ToString(), ГРН = documentXML.СвЮЛ.СвНаимЮЛ.ГРНДата.ГРН, ДатаЗаписи = documentXML.СвЮЛ.СвНаимЮЛ.ГРНДата.ДатаЗаписи, НаимЮЛПолн = documentXML.СвЮЛ.СвНаимЮЛ.НаимЮЛПолн, НаимЮЛСокр = documentXML.СвЮЛ.СвНаимЮЛ.СвНаимЮЛСокр?.НаимСокр, idЛицо = ULDB.Id };

                                lock (listLockers[6])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвНаимЮЛ_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[6])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвНаимЮЛ.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвНаимЮЛ_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвНаимЮЛ_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвНаимЮЛ).Load();

                                        if (ULDB.ЕГРЮЛ_СвНаимЮЛ?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвНаимЮЛ?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвНаимЮЛ_Delete.Add(ULDB.ЕГРЮЛ_СвНаимЮЛ?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[6])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвНаимЮЛ_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвНаимЮЛ.ГРНДата.ДатаЗаписи)
                                {
                                    foreach (var property in typeof(EGRULSvNaimUL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.Таблица = "EGRULSvNaimUL";
                                            changes.Столбец = property.Name;
                                            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                            changes.ИНН = documentXML.СвЮЛ.ИНН;
                                            changes.ДатаИзменения = DateTime.Now;

                                            IPSubTables.ИсторияИзменений_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвНаимЮЛ).Load();

                            //if (ULDB.ЕГРЮЛ_СвНаимЮЛ.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвНаимЮЛ.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвНаимЮЛ.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвНаимЮЛ = new List<EGRULSvNaimUL> { new EGRULSvNaimUL { Id = ULDB.ЕГРЮЛ_СвНаимЮЛ.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвНаимЮЛ.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ГРН = documentXML.СвЮЛ.СвНаимЮЛ.ГРНДата.ГРН, ДатаЗаписи = documentXML.СвЮЛ.СвНаимЮЛ.ГРНДата.ДатаЗаписи, НаимЮЛПолн = documentXML.СвЮЛ.СвНаимЮЛ.НаимЮЛПолн, НаимЮЛСокр = documentXML.СвЮЛ.СвНаимЮЛ.СвНаимЮЛСокр?.НаимСокр, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвНаимЮЛ.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvNaimUL).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвНаимЮЛ.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvNaimUL";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвНаимЮЛ.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвНаимЮЛ = new List<EGRULSvNaimUL> { new EGRULSvNaimUL { Id = Guid.NewGuid().ToString(), ГРН = documentXML.СвЮЛ.СвНаимЮЛ.ГРНДата.ГРН, ДатаЗаписи = documentXML.СвЮЛ.СвНаимЮЛ.ГРНДата.ДатаЗаписи, НаимЮЛПолн = documentXML.СвЮЛ.СвНаимЮЛ.НаимЮЛПолн, НаимЮЛСокр = documentXML.СвЮЛ.СвНаимЮЛ.СвНаимЮЛСокр?.НаимСокр, idЛицо = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvObrUL":
                            if (documentXML.СвЮЛ.СвОбрЮЛ == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvObrUL previousEntry;
                                var newEntry = new EGRULSvObrUL { Id = Guid.NewGuid().ToString(), ДатаОГРН = documentXML.СвЮЛ.СвОбрЮЛ.ДатаОГРН, ОГРН = documentXML.СвЮЛ.СвОбрЮЛ.ОГРН, НаимСпОбрЮЛ = documentXML.СвЮЛ.СвОбрЮЛ.СпОбрЮЛ?.НаимСпОбрЮЛ, КодСпОбрЮЛ = documentXML.СвЮЛ.СвОбрЮЛ.СпОбрЮЛ?.КодСпОбрЮЛ, ДатаЗаписи = documentXML.СвЮЛ.СвОбрЮЛ.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвОбрЮЛ.ГРНДата.ГРН, ДатаРег = documentXML.СвЮЛ.СвОбрЮЛ.ДатаРег, НаимРО = documentXML.СвЮЛ.СвОбрЮЛ.НаимРО, РегНом = documentXML.СвЮЛ.СвОбрЮЛ.РегНом, idЛицо = ULDB.Id };

                                lock (listLockers[7])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвОбрЮЛ_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[7])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвОбрЮЛ.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвОбрЮЛ_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвОбрЮЛ_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвОбрЮЛ).Load();

                                        if (ULDB.ЕГРЮЛ_СвОбрЮЛ?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвОбрЮЛ?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвОбрЮЛ_Delete.Add(ULDB.ЕГРЮЛ_СвОбрЮЛ?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[7])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвОбрЮЛ_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвОбрЮЛ.ГРНДата.ДатаЗаписи)
                                {
                                    foreach (var property in typeof(EGRULSvObrUL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.Таблица = "EGRULSvObrUL";
                                            changes.Столбец = property.Name;
                                            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                            changes.ИНН = documentXML.СвЮЛ.ИНН;
                                            changes.ДатаИзменения = DateTime.Now;

                                            IPSubTables.ИсторияИзменений_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвОбрЮЛ).Load();

                            //if (ULDB.ЕГРЮЛ_СвОбрЮЛ.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвОбрЮЛ.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвОбрЮЛ.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвОбрЮЛ = new List<EGRULSvObrUL> { new EGRULSvObrUL { Id = ULDB.ЕГРЮЛ_СвОбрЮЛ.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвОбрЮЛ.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ДатаОГРН = documentXML.СвЮЛ.СвОбрЮЛ.ДатаОГРН, ОГРН = documentXML.СвЮЛ.СвОбрЮЛ.ОГРН, НаимСпОбрЮЛ = documentXML.СвЮЛ.СвОбрЮЛ.СпОбрЮЛ?.НаимСпОбрЮЛ, КодСпОбрЮЛ = documentXML.СвЮЛ.СвОбрЮЛ.СпОбрЮЛ?.КодСпОбрЮЛ, ДатаЗаписи = documentXML.СвЮЛ.СвОбрЮЛ.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвОбрЮЛ.ГРНДата.ГРН, ДатаРег = documentXML.СвЮЛ.СвОбрЮЛ.ДатаРег, НаимРО = documentXML.СвЮЛ.СвОбрЮЛ.НаимРО, РегНом = documentXML.СвЮЛ.СвОбрЮЛ.РегНом, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвОбрЮЛ.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvObrUL).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвОбрЮЛ.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvObrUL";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвОбрЮЛ.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвОбрЮЛ = new List<EGRULSvObrUL> { new EGRULSvObrUL { Id = Guid.NewGuid().ToString(), ДатаОГРН = documentXML.СвЮЛ.СвОбрЮЛ.ДатаОГРН, ОГРН = documentXML.СвЮЛ.СвОбрЮЛ.ОГРН, НаимСпОбрЮЛ = documentXML.СвЮЛ.СвОбрЮЛ.СпОбрЮЛ?.НаимСпОбрЮЛ, КодСпОбрЮЛ = documentXML.СвЮЛ.СвОбрЮЛ.СпОбрЮЛ?.КодСпОбрЮЛ, ДатаЗаписи = documentXML.СвЮЛ.СвОбрЮЛ.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвОбрЮЛ.ГРНДата.ГРН, ДатаРег = documentXML.СвЮЛ.СвОбрЮЛ.ДатаРег, НаимРО = documentXML.СвЮЛ.СвОбрЮЛ.НаимРО, РегНом = documentXML.СвЮЛ.СвОбрЮЛ.РегНом, idЛицо = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvPodrazd":
                            if (documentXML.СвЮЛ.СвПодразд == null)
                            {
                                continue;
                            }
                            else
                            {
                                List<EGRULSvPodrazd> previousEntries;
                                var currentEntries = new List<EGRULSvPodrazd>();
                                var entriesDates = documentXML.СвЮЛ.СвПодразд.СвФилиал.Select(d => d.АдрМНРФ?.ГРНДата.ДатаЗаписи).ToArray();
                                DateTime? currentEntriesDate = entriesDates.Count() > 0 ? entriesDates.Max() : DateTime.MinValue;

                                lock (listLockers[8])
                                {
                                    previousEntries = ULSubTables.ЕГРЮЛ_СвПодразд_Insert.Where(e => e.idЛицо == ULDB.Id).ToList();
                                }
                                if (previousEntries.Count != 0)
                                {
                                    lock (listLockers[8])
                                    {
                                        if (previousEntries.Select(d => d.ДатаЗаписи).ToArray()?.Max() <= currentEntriesDate)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвПодразд_Insert.RemoveAll(e => e.idЛицо == ULDB.Id);

                                            foreach (var svFilial in documentXML.СвЮЛ.СвПодразд.СвФилиал)
                                            {
                                                ULSubTables.ЕГРЮЛ_СвПодразд_Insert.Add(new EGRULSvPodrazd { Id = Guid.NewGuid().ToString(), НаимПолн = svFilial.СвНаим?.НаимПолн, Дом = svFilial.АдрМНРФ?.Дом, КодАдрКладр = svFilial.АдрМНРФ?.КодАдрКладр, КодРегион = svFilial.АдрМНРФ?.КодРегион, Индекс = svFilial.АдрМНРФ?.Индекс, НаимРегион = svFilial.АдрМНРФ?.Регион?.НаимРегион, НаимГород = svFilial.АдрМНРФ?.Город?.НаимГород, НаимУлица = svFilial.АдрМНРФ?.Улица?.НаимУлица, ТипГород = svFilial.АдрМНРФ?.Город?.ТипГород, ТипРегион = svFilial.АдрМНРФ?.Регион?.ТипРегион, ТипУлица = svFilial.АдрМНРФ?.Улица?.ТипУлица, ДатаЗаписи = svFilial.АдрМНРФ?.ГРНДата.ДатаЗаписи, ГРН = svFilial.АдрМНРФ?.ГРНДата.ГРН, idЛицо = ULDB.Id });
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвПодразд).Load();

                                        if (ULDB.ЕГРЮЛ_СвПодразд?.FirstOrDefault() != null)
                                        {
                                            previousEntries = ULDB.ЕГРЮЛ_СвПодразд.ToList();
                                            ULSubTables.ЕГРЮЛ_СвПодразд_Delete.Add(ULDB.ЕГРЮЛ_СвПодразд?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[8])
                                    {
                                        foreach (var svFilial in documentXML.СвЮЛ.СвПодразд.СвФилиал)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвПодразд_Insert.Add(new EGRULSvPodrazd { Id = Guid.NewGuid().ToString(), НаимПолн = svFilial.СвНаим?.НаимПолн, Дом = svFilial.АдрМНРФ?.Дом, КодАдрКладр = svFilial.АдрМНРФ?.КодАдрКладр, КодРегион = svFilial.АдрМНРФ?.КодРегион, Индекс = svFilial.АдрМНРФ?.Индекс, НаимРегион = svFilial.АдрМНРФ?.Регион?.НаимРегион, НаимГород = svFilial.АдрМНРФ?.Город?.НаимГород, НаимУлица = svFilial.АдрМНРФ?.Улица?.НаимУлица, ТипГород = svFilial.АдрМНРФ?.Город?.ТипГород, ТипРегион = svFilial.АдрМНРФ?.Регион?.ТипРегион, ТипУлица = svFilial.АдрМНРФ?.Улица?.ТипУлица, ДатаЗаписи = svFilial.АдрМНРФ?.ГРНДата.ДатаЗаписи, ГРН = svFilial.АдрМНРФ?.ГРНДата.ГРН, idЛицо = ULDB.Id });
                                        }
                                    }
                                }

                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвПодразд).Load();

                            //if (ULDB.ЕГРЮЛ_СвПодразд?.Count > 0)
                            //{
                            //    foreach (var entry in ULDB.ЕГРЮЛ_СвПодразд)
                            //    {
                            //        _dbcontext.Entry(entry).State = EntityState.Deleted;
                            //    }

                            //    //var previousEntries = ULDB.ЕГРЮЛ_СвПодразд;

                            //    ULDB.ЕГРЮЛ_СвПодразд = new List<EGRULSvPodrazd>();

                            //    foreach (var svFilial in documentXML.СвЮЛ.СвПодразд.СвФилиал)
                            //    {
                            //        ULDB.ЕГРЮЛ_СвПодразд.Add(new EGRULSvPodrazd { Id = Guid.NewGuid().ToString(), НаимПолн = svFilial.СвНаим?.НаимПолн, Дом = svFilial.АдрМНРФ?.Дом, КодАдрКладр = svFilial.АдрМНРФ?.КодАдрКладр, КодРегион = svFilial.АдрМНРФ?.КодРегион, Индекс = svFilial.АдрМНРФ?.Индекс, НаимРегион = svFilial.АдрМНРФ?.Регион?.НаимРегион, НаимГород = svFilial.АдрМНРФ?.Город?.НаимГород, НаимУлица = svFilial.АдрМНРФ?.Улица?.НаимУлица, ТипГород = svFilial.АдрМНРФ?.Город?.ТипГород, ТипРегион = svFilial.АдрМНРФ?.Регион?.ТипРегион, ТипУлица = svFilial.АдрМНРФ?.Улица?.ТипУлица, ДатаЗаписи = svFilial.АдрМНРФ?.ГРНДата.ДатаЗаписи, ГРН = svFilial.АдрМНРФ?.ГРНДата.ГРН, idЛицо = ULDB.Id });
                            //        _dbcontext.Entry(ULDB.ЕГРЮЛ_СвПодразд.Last()).State = EntityState.Added;
                            //    }

                            //    //var currentEntries = ULDB.ЕГРЮЛ_СвПодразд;

                            //    //foreach (var oldEntry in previousEntries)
                            //    //{
                            //    //    var newEntry = currentEntries.Where(e => e.НомЛиц == oldEntry.НомЛиц).FirstOrDefault();

                            //    //    if (newEntry == null)
                            //    //    {
                            //    //        continue;
                            //    //    }

                            //    //    foreach (var property in typeof(EGRULSvLicense).GetProperties())
                            //    //    {
                            //    //        if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                            //    //        {
                            //    //            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ИП")
                            //    //                continue;

                            //    //            ChangeLog changes = new ChangeLog();

                            //    //            changes.Таблица = "EGRULSvLicense";
                            //    //            changes.Столбец = property.Name;
                            //    //            changes.ЗначениеДо = property.GetValue(oldEntry)?.ToString();
                            //    //            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                            //    //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //    //            changes.ДатаИзменения = DateTime.Now;

                            //    //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //    //        }
                            //    //    }
                            //    //}
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвПодразд = new List<EGRULSvPodrazd>();

                            //    foreach (var svFilial in documentXML.СвЮЛ.СвПодразд.СвФилиал)
                            //    {
                            //        ULDB.ЕГРЮЛ_СвПодразд.Add(new EGRULSvPodrazd { Id = Guid.NewGuid().ToString(), НаимПолн = svFilial.СвНаим?.НаимПолн, Дом = svFilial.АдрМНРФ?.Дом, КодАдрКладр = svFilial.АдрМНРФ?.КодАдрКладр, КодРегион = svFilial.АдрМНРФ?.КодРегион, Индекс = svFilial.АдрМНРФ?.Индекс, НаимРегион = svFilial.АдрМНРФ?.Регион?.НаимРегион, НаимГород = svFilial.АдрМНРФ?.Город?.НаимГород, НаимУлица = svFilial.АдрМНРФ?.Улица?.НаимУлица, ТипГород = svFilial.АдрМНРФ?.Город?.ТипГород, ТипРегион = svFilial.АдрМНРФ?.Регион?.ТипРегион, ТипУлица = svFilial.АдрМНРФ?.Улица?.ТипУлица, ДатаЗаписи = svFilial.АдрМНРФ?.ГРНДата.ДатаЗаписи, ГРН = svFilial.АдрМНРФ?.ГРНДата.ГРН, idЛицо = ULDB.Id });
                            //    }
                            //}

                            break;

                        case "EGRULSvPredsh":
                            if (documentXML.СвЮЛ.СвПредш == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvPredsh previousEntry;
                                var newEntry = new EGRULSvPredsh { Id = Guid.NewGuid().ToString(), ИНН = documentXML.СвЮЛ.СвПредш.ИНН, ОГРН = documentXML.СвЮЛ.СвПредш.ОГРН, НаимЮЛПолн = documentXML.СвЮЛ.СвПредш.НаимЮЛПолн, ДатаЗаписи = documentXML.СвЮЛ.СвПредш.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвПредш.ГРНДата.ГРН, idЛицо = ULDB.Id };

                                lock (listLockers[9])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвПредш_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[9])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвПредш.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвПредш_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвПредш_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвПредш).Load();

                                        if (ULDB.ЕГРЮЛ_СвПредш?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвПредш?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвПредш_Delete.Add(ULDB.ЕГРЮЛ_СвПредш?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[9])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвПредш_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвПредш.ГРНДата.ДатаЗаписи)
                                {
                                    foreach (var property in typeof(EGRULSvPredsh).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.Таблица = "EGRULSvPredsh";
                                            changes.Столбец = property.Name;
                                            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                            changes.ИНН = documentXML.СвЮЛ.ИНН;
                                            changes.ДатаИзменения = DateTime.Now;

                                            IPSubTables.ИсторияИзменений_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвПредш).Load();

                            //if (ULDB.ЕГРЮЛ_СвПредш.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвПредш.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвПредш.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвПредш = new List<EGRULSvPredsh> { new EGRULSvPredsh { Id = ULDB.ЕГРЮЛ_СвПредш.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвПредш.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ИНН = documentXML.СвЮЛ.СвПредш.ИНН, ОГРН = documentXML.СвЮЛ.СвПредш.ОГРН, НаимЮЛПолн = documentXML.СвЮЛ.СвПредш.НаимЮЛПолн, ДатаЗаписи = documentXML.СвЮЛ.СвПредш.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвПредш.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвПредш.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvPredsh).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвПредш.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvPredsh";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвПредш.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвПредш = new List<EGRULSvPredsh> { new EGRULSvPredsh { Id = Guid.NewGuid().ToString(), ИНН = documentXML.СвЮЛ.СвПредш.ИНН, ОГРН = documentXML.СвЮЛ.СвПредш.ОГРН, НаимЮЛПолн = documentXML.СвЮЛ.СвПредш.НаимЮЛПолн, ДатаЗаписи = documentXML.СвЮЛ.СвПредш.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвПредш.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvPreem":
                            if (documentXML.СвЮЛ.СвПреем == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvPreem previousEntry;
                                var newEntry = new EGRULSvPreem { Id = Guid.NewGuid().ToString(), ИНН = documentXML.СвЮЛ.СвПреем.ИНН, ОГРН = documentXML.СвЮЛ.СвПреем.ОГРН, НаимЮЛПолн = documentXML.СвЮЛ.СвПреем.НаимЮЛПолн, ДатаЗаписи = documentXML.СвЮЛ.СвПреем.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвПреем.ГРНДата.ГРН, idЛицо = ULDB.Id };

                                lock (listLockers[10])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвПреем_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[10])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвПреем.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвПреем_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвПреем_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвПреем).Load();

                                        if (ULDB.ЕГРЮЛ_СвПреем?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвПреем?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвПреем_Delete.Add(ULDB.ЕГРЮЛ_СвПреем?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[10])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвПреем_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвПреем.ГРНДата.ДатаЗаписи)
                                {
                                    foreach (var property in typeof(EGRULSvPreem).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.Таблица = "EGRULSvPreem";
                                            changes.Столбец = property.Name;
                                            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                            changes.ИНН = documentXML.СвЮЛ.ИНН;
                                            changes.ДатаИзменения = DateTime.Now;

                                            IPSubTables.ИсторияИзменений_Insert.Add(changes);
                                        }
                                    }
                                }
                            }
                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвПреем).Load();

                            //if (ULDB.ЕГРЮЛ_СвПреем.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвПреем.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвПреем.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвПреем = new List<EGRULSvPreem> { new EGRULSvPreem { Id = ULDB.ЕГРЮЛ_СвПреем.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвПреем.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ИНН = documentXML.СвЮЛ.СвПреем.ИНН, ОГРН = documentXML.СвЮЛ.СвПреем.ОГРН, НаимЮЛПолн = documentXML.СвЮЛ.СвПреем.НаимЮЛПолн, ДатаЗаписи = documentXML.СвЮЛ.СвПреем.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвПреем.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвПреем.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvPreem).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвПреем.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvPreem";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвПреем.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвПреем = new List<EGRULSvPreem> { new EGRULSvPreem { Id = Guid.NewGuid().ToString(), ИНН = documentXML.СвЮЛ.СвПреем.ИНН, ОГРН = documentXML.СвЮЛ.СвПреем.ОГРН, НаимЮЛПолн = documentXML.СвЮЛ.СвПреем.НаимЮЛПолн, ДатаЗаписи = documentXML.СвЮЛ.СвПреем.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвПреем.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvPrekrUL":
                            if (documentXML.СвЮЛ.СвПрекрЮЛ == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvPrekrUL previousEntry;
                                var newEntry = new EGRULSvPrekrUL { Id = Guid.NewGuid().ToString(), ДатаПрекрЮЛ = documentXML.СвЮЛ.СвПрекрЮЛ.СпПрекрЮЛ?.ДатаПрекрЮЛ, КодНО = documentXML.СвЮЛ.СвПрекрЮЛ.СвРегОрг?.КодНО, НаимНО = documentXML.СвЮЛ.СвПрекрЮЛ.СвРегОрг?.НаимНО, ГРН = documentXML.СвЮЛ.СвПрекрЮЛ.ГРНДата.ГРН, ДатаЗаписи = documentXML.СвЮЛ.СвПрекрЮЛ.ГРНДата.ДатаЗаписи, idЛицо = ULDB.Id };

                                lock (listLockers[11])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвПрекрЮЛ_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[11])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвПрекрЮЛ.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвПрекрЮЛ_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвПрекрЮЛ_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвПрекрЮЛ).Load();

                                        if (ULDB.ЕГРЮЛ_СвПрекрЮЛ?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвПрекрЮЛ?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвПрекрЮЛ_Delete.Add(ULDB.ЕГРЮЛ_СвПрекрЮЛ?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[11])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвПрекрЮЛ_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвПрекрЮЛ.ГРНДата.ДатаЗаписи)
                                {
                                    foreach (var property in typeof(EGRULSvPrekrUL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.Таблица = "EGRULSvPrekrUL";
                                            changes.Столбец = property.Name;
                                            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                            changes.ИНН = documentXML.СвЮЛ.ИНН;
                                            changes.ДатаИзменения = DateTime.Now;

                                            IPSubTables.ИсторияИзменений_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвПрекрЮЛ).Load();

                            //if (ULDB.ЕГРЮЛ_СвПрекрЮЛ.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвПрекрЮЛ.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвПрекрЮЛ.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвПрекрЮЛ = new List<EGRULSvPrekrUL> { new EGRULSvPrekrUL { Id = ULDB.ЕГРЮЛ_СвПрекрЮЛ.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвПрекрЮЛ.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ДатаПрекрЮЛ = documentXML.СвЮЛ.СвПрекрЮЛ.СпПрекрЮЛ?.ДатаПрекрЮЛ, КодНО = documentXML.СвЮЛ.СвПрекрЮЛ.СвРегОрг?.КодНО, НаимНО = documentXML.СвЮЛ.СвПрекрЮЛ.СвРегОрг?.НаимНО, ГРН = documentXML.СвЮЛ.СвПрекрЮЛ.ГРНДата.ГРН, ДатаЗаписи = documentXML.СвЮЛ.СвПрекрЮЛ.ГРНДата.ДатаЗаписи, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвПрекрЮЛ.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvPrekrUL).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвПрекрЮЛ.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvPrekrUL";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвПрекрЮЛ.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвПрекрЮЛ = new List<EGRULSvPrekrUL> { new EGRULSvPrekrUL { Id = Guid.NewGuid().ToString(), ДатаПрекрЮЛ = documentXML.СвЮЛ.СвПрекрЮЛ.СпПрекрЮЛ?.ДатаПрекрЮЛ, КодНО = documentXML.СвЮЛ.СвПрекрЮЛ.СвРегОрг?.КодНО, НаимНО = documentXML.СвЮЛ.СвПрекрЮЛ.СвРегОрг?.НаимНО, ГРН = documentXML.СвЮЛ.СвПрекрЮЛ.ГРНДата.ГРН, ДатаЗаписи = documentXML.СвЮЛ.СвПрекрЮЛ.ГРНДата.ДатаЗаписи, idЛицо = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvRegOrg":
                            if (documentXML.СвЮЛ.СвРегОрг == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvRegOrg previousEntry;
                                var newEntry = new EGRULSvRegOrg { Id = Guid.NewGuid().ToString(), АдрРО = documentXML.СвЮЛ.СвРегОрг?.АдрРО, НаимНО = documentXML.СвЮЛ.СвРегОрг?.НаимНО, КодНО = documentXML.СвЮЛ.СвРегОрг?.КодНО, ДатаЗаписи = documentXML.СвЮЛ.СвРегОрг?.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвРегОрг?.ГРНДата.ГРН, idЛицо = ULDB.Id };

                                lock (listLockers[12])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвРегОрг_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[12])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвРегОрг.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвРегОрг_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвРегОрг_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвРегОрг).Load();

                                        if (ULDB.ЕГРЮЛ_СвРегОрг?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвРегОрг?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвРегОрг_Delete.Add(ULDB.ЕГРЮЛ_СвРегОрг?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[12])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвРегОрг_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвРегОрг.ГРНДата.ДатаЗаписи)
                                {
                                    foreach (var property in typeof(EGRULSvRegOrg).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.Таблица = "EGRULSvRegOrg";
                                            changes.Столбец = property.Name;
                                            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                            changes.ИНН = documentXML.СвЮЛ.ИНН;
                                            changes.ДатаИзменения = DateTime.Now;

                                            IPSubTables.ИсторияИзменений_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвРегОрг).Load();

                            //if (ULDB.ЕГРЮЛ_СвРегОрг?.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвРегОрг?.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвРегОрг?.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвРегОрг = new List<EGRULSvRegOrg> { new EGRULSvRegOrg { Id = ULDB.ЕГРЮЛ_СвРегОрг?.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвРегОрг?.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), АдрРО = documentXML.СвЮЛ.СвРегОрг?.АдрРО, НаимНО = documentXML.СвЮЛ.СвРегОрг?.НаимНО, КодНО = documentXML.СвЮЛ.СвРегОрг?.КодНО, ДатаЗаписи = documentXML.СвЮЛ.СвРегОрг?.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвРегОрг?.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвРегОрг?.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvRegOrg).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвРегОрг?.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvRegOrg";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвРегОрг?.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвРегОрг = new List<EGRULSvRegOrg> { new EGRULSvRegOrg { Id = Guid.NewGuid().ToString(), АдрРО = documentXML.СвЮЛ.СвРегОрг?.АдрРО, НаимНО = documentXML.СвЮЛ.СвРегОрг?.НаимНО, КодНО = documentXML.СвЮЛ.СвРегОрг?.КодНО, ДатаЗаписи = documentXML.СвЮЛ.СвРегОрг?.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвРегОрг?.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvRegPF":
                            if (documentXML.СвЮЛ.СвРегПФ == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvRegPF previousEntry;
                                var newEntry = new EGRULSvRegPF { Id = Guid.NewGuid().ToString(), ДатаРег = documentXML.СвЮЛ.СвРегПФ.ДатаРег, РегНомПФ = documentXML.СвЮЛ.СвРегПФ.РегНомПФ, НаимПФ = documentXML.СвЮЛ.СвРегПФ.СвОргПФ?.НаимПФ, КодПФ = documentXML.СвЮЛ.СвРегПФ.СвОргПФ?.КодПФ, ДатаЗаписи = documentXML.СвЮЛ.СвРегПФ.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвРегПФ.ГРНДата.ГРН, idЛицо = ULDB.Id };

                                lock (listLockers[13])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвРегПФ_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[13])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвРегПФ.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвРегПФ_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвРегПФ_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвРегПФ).Load();

                                        if (ULDB.ЕГРЮЛ_СвРегПФ?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвРегПФ?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвРегПФ_Delete.Add(ULDB.ЕГРЮЛ_СвРегПФ?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[13])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвРегПФ_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвРегПФ.ГРНДата.ДатаЗаписи)
                                {
                                    foreach (var property in typeof(EGRULSvRegPF).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.Таблица = "EGRULSvRegPF";
                                            changes.Столбец = property.Name;
                                            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                            changes.ИНН = documentXML.СвЮЛ.ИНН;
                                            changes.ДатаИзменения = DateTime.Now;

                                            IPSubTables.ИсторияИзменений_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвРегПФ).Load();

                            //if (ULDB.ЕГРЮЛ_СвРегПФ.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвРегПФ.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвРегПФ.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвРегПФ = new List<EGRULSvRegPF> { new EGRULSvRegPF { Id = ULDB.ЕГРЮЛ_СвРегПФ.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвРегПФ.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ДатаРег = documentXML.СвЮЛ.СвРегПФ.ДатаРег, РегНомПФ = documentXML.СвЮЛ.СвРегПФ.РегНомПФ, НаимПФ = documentXML.СвЮЛ.СвРегПФ.СвОргПФ?.НаимПФ, КодПФ = documentXML.СвЮЛ.СвРегПФ.СвОргПФ?.КодПФ, ДатаЗаписи = documentXML.СвЮЛ.СвРегПФ.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвРегПФ.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвРегПФ.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvRegPF).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвРегПФ.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvRegPF";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвРегПФ.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвРегПФ = new List<EGRULSvRegPF> { new EGRULSvRegPF { Id = Guid.NewGuid().ToString(), ДатаРег = documentXML.СвЮЛ.СвРегПФ.ДатаРег, РегНомПФ = documentXML.СвЮЛ.СвРегПФ.РегНомПФ, НаимПФ = documentXML.СвЮЛ.СвРегПФ.СвОргПФ?.НаимПФ, КодПФ = documentXML.СвЮЛ.СвРегПФ.СвОргПФ?.КодПФ, ДатаЗаписи = documentXML.СвЮЛ.СвРегПФ.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвРегПФ.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvRegFSS":
                            if (documentXML.СвЮЛ.СвРегФСС == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvRegFSS previousEntry;
                                var newEntry = new EGRULSvRegFSS { Id = Guid.NewGuid().ToString(), ДатаРег = documentXML.СвЮЛ.СвРегФСС?.ДатаРег, РегНомФСС = documentXML.СвЮЛ.СвРегФСС?.РегНомФСС, НаимФСС = documentXML.СвЮЛ.СвРегФСС?.СвОргФСС?.НаимФСС, КодФСС = documentXML.СвЮЛ.СвРегФСС?.СвОргФСС?.КодФСС, ДатаЗаписи = documentXML.СвЮЛ.СвРегФСС?.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвРегФСС?.ГРНДата.ГРН, idЛицо = ULDB.Id };

                                lock (listLockers[14])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвРегФСС_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[14])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвРегФСС.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвРегФСС_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвРегФСС_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвРегФСС).Load();

                                        if (ULDB.ЕГРЮЛ_СвРегФСС?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвРегФСС?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвРегФСС_Delete.Add(ULDB.ЕГРЮЛ_СвРегФСС?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[14])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвРегФСС_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвРегФСС.ГРНДата.ДатаЗаписи)
                                {
                                    foreach (var property in typeof(EGRULSvRegFSS).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.Таблица = "EGRULSvRegFSS";
                                            changes.Столбец = property.Name;
                                            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                            changes.ИНН = documentXML.СвЮЛ.ИНН;
                                            changes.ДатаИзменения = DateTime.Now;

                                            IPSubTables.ИсторияИзменений_Insert.Add(changes);
                                        }
                                    }
                                }
                            }
                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвРегФСС).Load();

                            //if (ULDB.ЕГРЮЛ_СвРегФСС.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвРегФСС.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвРегФСС.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвРегФСС = new List<EGRULSvRegFSS> { new EGRULSvRegFSS { Id = ULDB.ЕГРЮЛ_СвРегФСС.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвРегФСС.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ДатаРег = documentXML.СвЮЛ.СвРегФСС?.ДатаРег, РегНомФСС = documentXML.СвЮЛ.СвРегФСС?.РегНомФСС, НаимФСС = documentXML.СвЮЛ.СвРегФСС?.СвОргФСС?.НаимФСС, КодФСС = documentXML.СвЮЛ.СвРегФСС?.СвОргФСС?.КодФСС, ДатаЗаписи = documentXML.СвЮЛ.СвРегФСС?.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвРегФСС?.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвРегФСС.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvRegFSS).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвРегФСС.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvRegFSS";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвРегФСС.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвРегФСС = new List<EGRULSvRegFSS> { new EGRULSvRegFSS { Id = Guid.NewGuid().ToString(), ДатаРег = documentXML.СвЮЛ.СвРегФСС.ДатаРег } };
                            //}

                            break;

                        case "EGRULSvReorg":
                            if (documentXML.СвЮЛ.СвРеорг == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvReorg previousEntry;
                                var newEntry = new EGRULSvReorg { Id = Guid.NewGuid().ToString(), ИНН = documentXML.СвЮЛ.СвРеорг.СвРеоргЮЛ?.ИНН, ОГРН = documentXML.СвЮЛ.СвРеорг.СвРеоргЮЛ?.ОГРН, КодСтатусЮЛ = documentXML.СвЮЛ.СвРеорг.СвСтатус?.КодСтатус, НаимСтатусЮЛ = documentXML.СвЮЛ.СвРеорг.СвСтатус?.НаимСтатус, НаимЮЛПолн = documentXML.СвЮЛ.СвРеорг.СвРеоргЮЛ?.НаимЮЛПолн, СостЮЛпосле = documentXML.СвЮЛ.СвРеорг.СвРеоргЮЛ?.СостЮЛпосле, ДатаЗаписи = documentXML.СвЮЛ.СвРеорг.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвРеорг.ГРНДата.ГРН, idЛицо = ULDB.Id };

                                lock (listLockers[15])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвРеорг_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[15])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвРеорг.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвРеорг_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвРеорг_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвРеорг).Load();

                                        if (ULDB.ЕГРЮЛ_СвРеорг?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвРеорг?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвРеорг_Delete.Add(ULDB.ЕГРЮЛ_СвРеорг?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[15])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвРеорг_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвРеорг.ГРНДата.ДатаЗаписи)
                                {
                                    foreach (var property in typeof(EGRULSvReorg).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.Таблица = "EGRULSvReorg";
                                            changes.Столбец = property.Name;
                                            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                            changes.ИНН = documentXML.СвЮЛ.ИНН;
                                            changes.ДатаИзменения = DateTime.Now;

                                            IPSubTables.ИсторияИзменений_Insert.Add(changes);
                                        }
                                    }
                                }
                            }
                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвРеорг).Load();

                            //if (ULDB.ЕГРЮЛ_СвРеорг.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвРеорг.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвРеорг.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвРеорг = new List<EGRULSvReorg> { new EGRULSvReorg { Id = ULDB.ЕГРЮЛ_СвРеорг.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвРеорг.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ИНН = documentXML.СвЮЛ.СвРеорг.СвРеоргЮЛ?.ИНН, ОГРН = documentXML.СвЮЛ.СвРеорг.СвРеоргЮЛ?.ОГРН, КодСтатусЮЛ = documentXML.СвЮЛ.СвРеорг.СвСтатус?.КодСтатус, НаимСтатусЮЛ = documentXML.СвЮЛ.СвРеорг.СвСтатус?.НаимСтатус, НаимЮЛПолн = documentXML.СвЮЛ.СвРеорг.СвРеоргЮЛ?.НаимЮЛПолн, СостЮЛпосле = documentXML.СвЮЛ.СвРеорг.СвРеоргЮЛ?.СостЮЛпосле, ДатаЗаписи = documentXML.СвЮЛ.СвРеорг.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвРеорг.ГРНДата.ГРН } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвРеорг.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvReorg).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвРеорг.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvReorg";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвРеорг.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвРеорг = new List<EGRULSvReorg> { new EGRULSvReorg { Id = Guid.NewGuid().ToString(), ИНН = documentXML.СвЮЛ.СвРеорг.СвРеоргЮЛ?.ИНН, ОГРН = documentXML.СвЮЛ.СвРеорг.СвРеоргЮЛ?.ОГРН, КодСтатусЮЛ = documentXML.СвЮЛ.СвРеорг.СвСтатус?.КодСтатус, НаимСтатусЮЛ = documentXML.СвЮЛ.СвРеорг.СвСтатус?.НаимСтатус, НаимЮЛПолн = documentXML.СвЮЛ.СвРеорг.СвРеоргЮЛ?.НаимЮЛПолн, СостЮЛпосле = documentXML.СвЮЛ.СвРеорг.СвРеоргЮЛ?.СостЮЛпосле, ДатаЗаписи = documentXML.СвЮЛ.СвРеорг.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвРеорг.ГРНДата.ГРН } };
                            //}

                            break;

                        case "EGRULSvStatus":
                            if (documentXML.СвЮЛ.СвСтатусКорневой == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvStatus previousEntry;
                                var newEntry = new EGRULSvStatus { Id = Guid.NewGuid().ToString(), НаимСтатусЮЛ = documentXML.СвЮЛ.СвСтатусКорневой.СвСтатус?.НаимСтатус, КодСтатусЮЛ = documentXML.СвЮЛ.СвСтатусКорневой.СвСтатус?.КодСтатус, НомерЖурнала = documentXML.СвЮЛ.СвСтатусКорневой.СвРешИсклЮЛ?.НомерЖурнала, ДатаПубликации = documentXML.СвЮЛ.СвСтатусКорневой.СвРешИсклЮЛ?.ДатаПубликации, НомерРеш = documentXML.СвЮЛ.СвСтатусКорневой.СвРешИсклЮЛ?.НомерРеш, ДатаРеш = documentXML.СвЮЛ.СвСтатусКорневой.СвРешИсклЮЛ?.ДатаРеш, ДатаЗаписи = documentXML.СвЮЛ.СвСтатусКорневой.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвСтатусКорневой.ГРНДата.ГРН, idЛицо = ULDB.Id };

                                lock (listLockers[17])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвСтатус_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[17])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвСтатусКорневой.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвСтатус_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвСтатус_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвСтатус).Load();

                                        if (ULDB.ЕГРЮЛ_СвСтатус?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвСтатус?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвСтатус_Delete.Add(ULDB.ЕГРЮЛ_СвСтатус?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[17])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвСтатус_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвСтатусКорневой.ГРНДата.ДатаЗаписи)
                                {
                                    foreach (var property in typeof(EGRULSvStatus).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.Таблица = "EGRULSvStatus";
                                            changes.Столбец = property.Name;
                                            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                            changes.ИНН = documentXML.СвЮЛ.ИНН;
                                            changes.ДатаИзменения = DateTime.Now;

                                            IPSubTables.ИсторияИзменений_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвСтатус).Load();

                            //if (ULDB.ЕГРЮЛ_СвСтатус?.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвСтатус?.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвСтатус?.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвСтатус = new List<EGRULSvStatus> { new EGRULSvStatus { Id = ULDB.ЕГРЮЛ_СвСтатус?.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвСтатус?.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), НаимСтатусЮЛ = documentXML.СвЮЛ.СвСтатусКорневой.СвСтатус?.НаимСтатус, КодСтатусЮЛ = documentXML.СвЮЛ.СвСтатусКорневой.СвСтатус?.КодСтатус, НомерЖурнала = documentXML.СвЮЛ.СвСтатусКорневой.СвРешИсклЮЛ?.НомерЖурнала, ДатаПубликации = documentXML.СвЮЛ.СвСтатусКорневой.СвРешИсклЮЛ?.ДатаПубликации, НомерРеш = documentXML.СвЮЛ.СвСтатусКорневой.СвРешИсклЮЛ?.НомерРеш, ДатаРеш = documentXML.СвЮЛ.СвСтатусКорневой.СвРешИсклЮЛ?.ДатаРеш, ДатаЗаписи = documentXML.СвЮЛ.СвСтатусКорневой.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвСтатусКорневой.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвСтатус?.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvStatus).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвСтатус?.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvStatus";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвСтатус?.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвСтатус = new List<EGRULSvStatus> { new EGRULSvStatus { Id = Guid.NewGuid().ToString(), НаимСтатусЮЛ = documentXML.СвЮЛ.СвСтатусКорневой.СвСтатус?.НаимСтатус, КодСтатусЮЛ = documentXML.СвЮЛ.СвСтатусКорневой.СвСтатус?.КодСтатус, НомерЖурнала = documentXML.СвЮЛ.СвСтатусКорневой.СвРешИсклЮЛ?.НомерЖурнала, ДатаПубликации = documentXML.СвЮЛ.СвСтатусКорневой.СвРешИсклЮЛ?.ДатаПубликации, НомерРеш = documentXML.СвЮЛ.СвСтатусКорневой.СвРешИсклЮЛ?.НомерРеш, ДатаРеш = documentXML.СвЮЛ.СвСтатусКорневой.СвРешИсклЮЛ?.ДатаРеш, ДатаЗаписи = documentXML.СвЮЛ.СвСтатусКорневой.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвСтатусКорневой.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvUstKap":
                            if (documentXML.СвЮЛ.СвУстКап == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvUstKap previousEntry;
                                var newEntry = new EGRULSvUstKap { Id = Guid.NewGuid().ToString(), НаимВидКап = documentXML.СвЮЛ.СвУстКап.НаимВидКап, СумКап = documentXML.СвЮЛ.СвУстКап.СумКап, ДатаЗаписи = documentXML.СвЮЛ.СвУстКап.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвУстКап.ГРНДата.ГРН, idЛицо = ULDB.Id };

                                lock (listLockers[18])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвУстКап_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[18])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвУстКап.ГРНДата.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвУстКап_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвУстКап_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвУстКап).Load();

                                        if (ULDB.ЕГРЮЛ_СвУстКап?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвУстКап?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвУстКап_Delete.Add(ULDB.ЕГРЮЛ_СвУстКап?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[18])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвУстКап_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СвУстКап.ГРНДата.ДатаЗаписи)
                                {
                                    foreach (var property in typeof(EGRULSvUstKap).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.Таблица = "EGRULSvUstKap";
                                            changes.Столбец = property.Name;
                                            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                            changes.ИНН = documentXML.СвЮЛ.ИНН;
                                            changes.ДатаИзменения = DateTime.Now;

                                            IPSubTables.ИсторияИзменений_Insert.Add(changes);
                                        }
                                    }
                                }
                            }
                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвУстКап).Load();

                            //if (ULDB.ЕГРЮЛ_СвУстКап.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвУстКап.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвУстКап.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвУстКап = new List<EGRULSvUstKap> { new EGRULSvUstKap { Id = ULDB.ЕГРЮЛ_СвУстКап.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвУстКап.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), НаимВидКап = documentXML.СвЮЛ.СвУстКап.НаимВидКап, СумКап = documentXML.СвЮЛ.СвУстКап.СумКап, ДатаЗаписи = documentXML.СвЮЛ.СвУстКап.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвУстКап.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвУстКап.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvUstKap).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвУстКап.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvUstKap";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвУстКап.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвУстКап = new List<EGRULSvUstKap> { new EGRULSvUstKap { Id = Guid.NewGuid().ToString(), НаимВидКап = documentXML.СвЮЛ.СвУстКап.НаимВидКап, СумКап = documentXML.СвЮЛ.СвУстКап.СумКап, ДатаЗаписи = documentXML.СвЮЛ.СвУстКап.ГРНДата.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СвУстКап.ГРНДата.ГРН, idЛицо = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvFounder":
                            if (documentXML.СвЮЛ.СвУчредит == null)
                            {
                                continue;
                            }
                            else
                            {
                                List<EGRULSvFounder> previousEntries;
                                var currentEntries = new List<EGRULSvFounder>();
                                var founderDates = new[] { documentXML.СвЮЛ.СвУчредит.УчрФЛ.Where(x => x.ГРНДатаПерв != null).Select(d => d.ГРНДатаПерв.ДатаЗаписи), documentXML.СвЮЛ.СвУчредит.УчрЮЛРос.Where(x => x.ГРНДатаПерв != null).Select(d => d.ГРНДатаПерв.ДатаЗаписи) }.SelectMany(date => date).ToArray();
                                DateTime? currentEntriesDate = founderDates.Count() > 0 ? founderDates.Max() : DateTime.MinValue;

                                lock (listLockers[19])
                                {
                                    previousEntries = ULSubTables.ЕГРЮЛ_СвУчредит_Insert.Where(e => e.idЛицо == ULDB.Id).ToList();

                                    if (previousEntries.Count != 0)
                                    {
                                        if (previousEntries.Select(d => d.ДатаЗаписи).ToArray()?.Max() <= currentEntriesDate)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвУчредит_Insert.RemoveAll(e => e.idЛицо == ULDB.Id);

                                            foreach (var svFL in documentXML.СвЮЛ.СвУчредит.УчрФЛ)
                                            {
                                                ULSubTables.ЕГРЮЛ_СвУчредит_Insert.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ДатаЗаписи = svFL.ГРНДатаПерв?.ДатаЗаписи, ГРН = svFL.ГРНДатаПерв?.ГРН, Отчество = svFL.СвФЛ?.Отчество, Имя = svFL.СвФЛ?.Имя, Фамилия = svFL.СвФЛ?.Фамилия, ИННФЛ = svFL.СвФЛ?.ИННФЛ, НоминСтоим = svFL.ДоляУстКап?.НоминСтоим, РазмерДоли = svFL.ДоляУстКап?.РазмерДоли?.Процент, idЛицо = ULDB.Id });
                                            }

                                            foreach (var svUL in documentXML.СвЮЛ.СвУчредит.УчрЮЛРос)
                                            {
                                                ULSubTables.ЕГРЮЛ_СвУчредит_Insert.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ДатаЗаписи = svUL.ГРНДатаПерв?.ДатаЗаписи, ГРН = svUL.ГРНДатаПерв?.ГРН, ИНН = svUL.НаимИННЮЛ?.ИНН, ОГРН = svUL.НаимИННЮЛ?.ОГРН, НаимЮЛПолн = svUL.НаимИННЮЛ?.НаимЮЛПолн, ТекстНедДанУчр = svUL.СвНедДанУчр?.ТекстНедДанУчр, ПризнНедДанУчр = svUL.СвНедДанУчр?.ПризнНедДанУчр, НоминСтоим = svUL.ДоляУстКап?.НоминСтоим, РазмерДоли = svUL.ДоляУстКап?.РазмерДоли?.Процент, idЛицо = ULDB.Id });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (firstTime == false)
                                        {
                                            _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвУчредит).Load();

                                            if (ULDB.ЕГРЮЛ_СвУчредит?.FirstOrDefault() != null)
                                            {
                                                previousEntries = ULDB.ЕГРЮЛ_СвУчредит.ToList();
                                                ULSubTables.ЕГРЮЛ_СвУчредит_Delete.Add(ULDB.ЕГРЮЛ_СвУчредит?.FirstOrDefault().idЛицо.ToString());
                                            }
                                        }

                                        foreach (var svFL in documentXML.СвЮЛ.СвУчредит.УчрФЛ)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвУчредит_Insert.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ДатаЗаписи = svFL.ГРНДатаПерв?.ДатаЗаписи, ГРН = svFL.ГРНДатаПерв?.ГРН, Отчество = svFL.СвФЛ?.Отчество, Имя = svFL.СвФЛ?.Имя, Фамилия = svFL.СвФЛ?.Фамилия, ИННФЛ = svFL.СвФЛ?.ИННФЛ, НоминСтоим = svFL.ДоляУстКап?.НоминСтоим, РазмерДоли = svFL.ДоляУстКап?.РазмерДоли?.Процент, idЛицо = ULDB.Id });
                                        }

                                        foreach (var svUL in documentXML.СвЮЛ.СвУчредит.УчрЮЛРос)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвУчредит_Insert.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ДатаЗаписи = svUL.ГРНДатаПерв?.ДатаЗаписи, ГРН = svUL.ГРНДатаПерв?.ГРН, ИНН = svUL.НаимИННЮЛ?.ИНН, ОГРН = svUL.НаимИННЮЛ?.ОГРН, НаимЮЛПолн = svUL.НаимИННЮЛ?.НаимЮЛПолн, ТекстНедДанУчр = svUL.СвНедДанУчр?.ТекстНедДанУчр, ПризнНедДанУчр = svUL.СвНедДанУчр?.ПризнНедДанУчр, НоминСтоим = svUL.ДоляУстКап?.НоминСтоим, РазмерДоли = svUL.ДоляУстКап?.РазмерДоли?.Процент, idЛицо = ULDB.Id });
                                        }
                                    }
                                }

                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвУчредит).Load();

                            //if (ULDB.ЕГРЮЛ_СвУчредит?.Count > 0)
                            //{
                            //    foreach (var entry in ULDB.ЕГРЮЛ_СвУчредит)
                            //    {
                            //        _dbcontext.Entry(entry).State = EntityState.Deleted;
                            //    }

                            //    var previousEntries = ULDB.ЕГРЮЛ_СвУчредит;

                            //    ULDB.ЕГРЮЛ_СвУчредит = new List<EGRULSvFounder>();

                            //    foreach (var svFL in documentXML.СвЮЛ.СвУчредит.УчрФЛ)
                            //    {
                            //        ULDB.ЕГРЮЛ_СвУчредит.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ДатаЗаписи = svFL.ГРНДатаПерв?.ДатаЗаписи, ГРН = svFL.ГРНДатаПерв?.ГРН, Отчество = svFL.СвФЛ?.Отчество, Имя = svFL.СвФЛ?.Имя, Фамилия = svFL.СвФЛ?.Фамилия, ИННФЛ = svFL.СвФЛ?.ИННФЛ, НоминСтоим = svFL.ДоляУстКап?.НоминСтоим, РазмерДоли = svFL.ДоляУстКап?.РазмерДоли?.Процент, idЛицо = ULDB.Id });
                            //        _dbcontext.Entry(ULDB.ЕГРЮЛ_СвУчредит.Last()).State = EntityState.Added;
                            //    }

                            //    foreach (var svUL in documentXML.СвЮЛ.СвУчредит.УчрЮЛРос)
                            //    {
                            //        ULDB.ЕГРЮЛ_СвУчредит.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ДатаЗаписи = svUL.ГРНДатаПерв?.ДатаЗаписи, ГРН = svUL.ГРНДатаПерв?.ГРН, ИНН = svUL.НаимИННЮЛ?.ИНН, ОГРН = svUL.НаимИННЮЛ?.ОГРН, НаимЮЛПолн = svUL.НаимИННЮЛ?.НаимЮЛПолн, ТекстНедДанУчр = svUL.СвНедДанУчр?.ТекстНедДанУчр, ПризнНедДанУчр = svUL.СвНедДанУчр?.ПризнНедДанУчр, НоминСтоим = svUL.ДоляУстКап?.НоминСтоим, РазмерДоли = svUL.ДоляУстКап?.РазмерДоли?.Процент, idЛицо = ULDB.Id });
                            //        _dbcontext.Entry(ULDB.ЕГРЮЛ_СвУчредит.Last()).State = EntityState.Added;
                            //    }

                            //    var currentEntries = ULDB.ЕГРЮЛ_СвУчредит;

                            //    foreach (var oldEntry in previousEntries)
                            //    {
                            //        var newEntry = currentEntries.Where(e => e.ИННФЛ == oldEntry.ИННФЛ && e.ИННФЛ != null || e.ИНН == oldEntry.ИНН && e.ИНН != null).FirstOrDefault();

                            //        if (newEntry == null)
                            //        {
                            //            continue;
                            //        }

                            //        foreach (var property in typeof(EGRULSvFounder).GetProperties())
                            //        {
                            //            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                            //            {
                            //                if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                            //                    continue;

                            //                ChangeLog changes = new ChangeLog();

                            //                changes.Таблица = "EGRULSvFounder";
                            //                changes.Столбец = property.Name;
                            //                changes.ЗначениеДо = property.GetValue(oldEntry)?.ToString();
                            //                changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                            //                changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //                changes.ДатаИзменения = DateTime.Now;

                            //                //_dbcontext.ИсторияИзменений.Add(changes);
                            //            }
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвУчредит = new List<EGRULSvFounder>();

                            //    foreach (var svFL in documentXML.СвЮЛ.СвУчредит.УчрФЛ)
                            //    {
                            //        ULDB.ЕГРЮЛ_СвУчредит.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ДатаЗаписи = svFL.ГРНДатаПерв?.ДатаЗаписи, ГРН = svFL.ГРНДатаПерв?.ГРН, Отчество = svFL.СвФЛ?.Отчество, Имя = svFL.СвФЛ?.Имя, Фамилия = svFL.СвФЛ?.Фамилия, ИННФЛ = svFL.СвФЛ?.ИННФЛ, НоминСтоим = svFL.ДоляУстКап?.НоминСтоим, РазмерДоли = svFL.ДоляУстКап?.РазмерДоли?.Процент, idЛицо = ULDB.Id });
                            //    }

                            //    foreach (var svUL in documentXML.СвЮЛ.СвУчредит.УчрЮЛРос)
                            //    {
                            //        ULDB.ЕГРЮЛ_СвУчредит.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ДатаЗаписи = svUL.ГРНДатаПерв?.ДатаЗаписи, ГРН = svUL.ГРНДатаПерв?.ГРН, ИНН = svUL.НаимИННЮЛ?.ИНН, ОГРН = svUL.НаимИННЮЛ?.ОГРН, НаимЮЛПолн = svUL.НаимИННЮЛ?.НаимЮЛПолн, ТекстНедДанУчр = svUL.СвНедДанУчр?.ТекстНедДанУчр, ПризнНедДанУчр = svUL.СвНедДанУчр?.ПризнНедДанУчр, НоминСтоим = svUL.ДоляУстКап?.НоминСтоим, РазмерДоли = svUL.ДоляУстКап?.РазмерДоли?.Процент, idЛицо = ULDB.Id });
                            //    }
                            //}

                            break;

                        case "EGRULSvUL":
                            if (documentXML.СвЮЛ == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvUL previousEntry;
                                var newEntry = new EGRULSvUL { Id = Guid.NewGuid().ToString(), ПолнНаимОПФ = documentXML.СвЮЛ.ПолнНаимОПФ, КодОПФ = documentXML.СвЮЛ.КодОПФ, СпрОПФ = documentXML.СвЮЛ.СпрОПФ, КПП = documentXML.СвЮЛ.КПП, ИНН = documentXML.СвЮЛ.ИНН, ДатаОГРН = documentXML.СвЮЛ.ДатаОГРН, ОГРН = documentXML.СвЮЛ.ОГРН, ДатаВып = documentXML.СвЮЛ.ДатаВып, НаимСокр = (String.IsNullOrEmpty(documentXML.СвЮЛ.СвНаимЮЛ.СвНаимЮЛСокр?.НаимСокр)) ? documentXML.СвЮЛ.СвНаимЮЛ.НаимЮЛПолн : documentXML.СвЮЛ.СвНаимЮЛ.СвНаимЮЛСокр?.НаимСокр, ОКВЭДОсн = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.КодОКВЭД, idЛицо = ULDB.Id };

                                lock (listLockers[20])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СвЮЛ_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[20])
                                    {
                                        if (previousEntry.ДатаВып <= documentXML.СвЮЛ.ДатаВып)
                                        {
                                            ULSubTables.ЕГРЮЛ_СвЮЛ_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СвЮЛ_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвЮЛ).Load();

                                        if (ULDB.ЕГРЮЛ_СвЮЛ?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СвЮЛ?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СвЮЛ_Delete.Add(ULDB.ЕГРЮЛ_СвЮЛ?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[20])
                                    {
                                        ULSubTables.ЕГРЮЛ_СвЮЛ_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаВып <= documentXML.СвЮЛ.ДатаВып)
                                {
                                    foreach (var property in typeof(EGRULSvUL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.Таблица = "EGRULSvUL";
                                            changes.Столбец = property.Name;
                                            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                            changes.ИНН = documentXML.СвЮЛ.ИНН;
                                            changes.ДатаИзменения = DateTime.Now;

                                            IPSubTables.ИсторияИзменений_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СвЮЛ).Load();

                            //if (ULDB.ЕГРЮЛ_СвЮЛ.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СвЮЛ.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвЮЛ.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СвЮЛ = new List<EGRULSvUL> { new EGRULSvUL { Id = ULDB.ЕГРЮЛ_СвЮЛ.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СвЮЛ.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ПолнНаимОПФ = documentXML.СвЮЛ.ПолнНаимОПФ, КодОПФ = documentXML.СвЮЛ.КодОПФ, СпрОПФ = documentXML.СвЮЛ.СпрОПФ, КПП = documentXML.СвЮЛ.КПП, ИНН = documentXML.СвЮЛ.ИНН, ДатаОГРН = documentXML.СвЮЛ.ДатаОГРН, ОГРН = documentXML.СвЮЛ.ОГРН, ДатаВып = documentXML.СвЮЛ.ДатаВып, НаимСокр = (String.IsNullOrEmpty(documentXML.СвЮЛ.СвНаимЮЛ.СвНаимЮЛСокр?.НаимСокр)) ? documentXML.СвЮЛ.СвНаимЮЛ.НаимЮЛПолн : documentXML.СвЮЛ.СвНаимЮЛ.СвНаимЮЛСокр?.НаимСокр, ОКВЭДОсн = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.КодОКВЭД, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СвЮЛ.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvUL).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СвЮЛ.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvUL";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СвЮЛ.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СвЮЛ = new List<EGRULSvUL> { new EGRULSvUL { Id = Guid.NewGuid().ToString(), ПолнНаимОПФ = documentXML.СвЮЛ.ПолнНаимОПФ, КодОПФ = documentXML.СвЮЛ.КодОПФ, СпрОПФ = documentXML.СвЮЛ.СпрОПФ, КПП = documentXML.СвЮЛ.КПП, ИНН = documentXML.СвЮЛ.ИНН, ДатаОГРН = documentXML.СвЮЛ.ДатаОГРН, ОГРН = documentXML.СвЮЛ.ОГРН, ДатаВып = documentXML.СвЮЛ.ДатаВып, НаимСокр = (String.IsNullOrEmpty(documentXML.СвЮЛ.СвНаимЮЛ.СвНаимЮЛСокр?.НаимСокр)) ? documentXML.СвЮЛ.СвНаимЮЛ.НаимЮЛПолн : documentXML.СвЮЛ.СвНаимЮЛ.СвНаимЮЛСокр?.НаимСокр, ОКВЭДОсн = documentXML.СвЮЛ.СвОКВЭД?.СвОКВЭДОсн?.КодОКВЭД, idЛицо = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvedDoljnFL":
                            if (documentXML.СвЮЛ.СведДолжнФЛ == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvedDoljnFL previousEntry;
                                var newEntry = new EGRULSvedDoljnFL { Id = Guid.NewGuid().ToString(), ДатаЗаписи = documentXML.СвЮЛ.СведДолжнФЛ.ГРНДатаПерв?.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СведДолжнФЛ.ГРНДатаПерв?.ГРН, Отчество = documentXML.СвЮЛ.СведДолжнФЛ.СвФЛ?.Отчество, Имя = documentXML.СвЮЛ.СведДолжнФЛ.СвФЛ?.Имя, Фамилия = documentXML.СвЮЛ.СведДолжнФЛ.СвФЛ?.Фамилия, НаимДолжн = documentXML.СвЮЛ.СведДолжнФЛ.СвДолжн?.НаимДолжн, НаимВидДолжн = documentXML.СвЮЛ.СведДолжнФЛ.СвДолжн?.НаимВидДолжн, ИННФЛ = documentXML.СвЮЛ.СведДолжнФЛ.СвФЛ?.ИННФЛ, ВидДолжн = documentXML.СвЮЛ.СведДолжнФЛ.СвДолжн?.ВидДолжн, idЛицо = ULDB.Id };

                                lock (listLockers[21])
                                {
                                    previousEntry = ULSubTables.ЕГРЮЛ_СведДолжнФЛ_Insert.Where(e => e.idЛицо == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[21])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СведДолжнФЛ.ГРНДатаПерв?.ДатаЗаписи)
                                        {
                                            ULSubTables.ЕГРЮЛ_СведДолжнФЛ_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            ULSubTables.ЕГРЮЛ_СведДолжнФЛ_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СведДолжнФЛ).Load();

                                        if (ULDB.ЕГРЮЛ_СведДолжнФЛ?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.ЕГРЮЛ_СведДолжнФЛ?.FirstOrDefault();
                                            ULSubTables.ЕГРЮЛ_СведДолжнФЛ_Delete.Add(ULDB.ЕГРЮЛ_СведДолжнФЛ?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[21])
                                    {
                                        ULSubTables.ЕГРЮЛ_СведДолжнФЛ_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвЮЛ.СведДолжнФЛ.ГРНДатаПерв?.ДатаЗаписи)
                                {
                                    foreach (var property in typeof(EGRULSvedDoljnFL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ЮрЛицо")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.Таблица = "EGRULSvedDoljnFL";
                                            changes.Столбец = property.Name;
                                            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                            changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                            changes.ИНН = documentXML.СвЮЛ.ИНН;
                                            changes.ДатаИзменения = DateTime.Now;

                                            IPSubTables.ИсторияИзменений_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.ЕГРЮЛ_СведДолжнФЛ).Load();

                            //if (ULDB.ЕГРЮЛ_СведДолжнФЛ.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.ЕГРЮЛ_СведДолжнФЛ.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СведДолжнФЛ.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.ЕГРЮЛ_СведДолжнФЛ = new List<EGRULSvedDoljnFL> { new EGRULSvedDoljnFL { Id = ULDB.ЕГРЮЛ_СведДолжнФЛ.FirstOrDefault()?.Id != null ? ULDB.ЕГРЮЛ_СведДолжнФЛ.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ДатаЗаписи = documentXML.СвЮЛ.СведДолжнФЛ.ГРНДатаПерв?.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СведДолжнФЛ.ГРНДатаПерв?.ГРН, Отчество = documentXML.СвЮЛ.СведДолжнФЛ.СвФЛ?.Отчество, Имя = documentXML.СвЮЛ.СведДолжнФЛ.СвФЛ?.Имя, Фамилия = documentXML.СвЮЛ.СведДолжнФЛ.СвФЛ?.Фамилия, НаимДолжн = documentXML.СвЮЛ.СведДолжнФЛ.СвДолжн?.НаимДолжн, НаимВидДолжн = documentXML.СвЮЛ.СведДолжнФЛ.СвДолжн?.НаимВидДолжн, ИННФЛ = documentXML.СвЮЛ.СведДолжнФЛ.СвФЛ?.ИННФЛ, ВидДолжн = documentXML.СвЮЛ.СведДолжнФЛ.СвДолжн?.ВидДолжн, idЛицо = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.ЕГРЮЛ_СведДолжнФЛ.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvedDoljnFL).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.ЕГРЮЛ_СведДолжнФЛ.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRULSvedDoljnFL";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(ULDB.ЕГРЮЛ_СведДолжнФЛ.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвЮЛ.ИНН;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.ЕГРЮЛ_СведДолжнФЛ = new List<EGRULSvedDoljnFL> { new EGRULSvedDoljnFL { Id = Guid.NewGuid().ToString(), ДатаЗаписи = documentXML.СвЮЛ.СведДолжнФЛ.ГРНДатаПерв?.ДатаЗаписи, ГРН = documentXML.СвЮЛ.СведДолжнФЛ.ГРНДатаПерв?.ГРН, Отчество = documentXML.СвЮЛ.СведДолжнФЛ.СвФЛ?.Отчество, Имя = documentXML.СвЮЛ.СведДолжнФЛ.СвФЛ?.Имя, Фамилия = documentXML.СвЮЛ.СведДолжнФЛ.СвФЛ?.Фамилия, НаимДолжн = documentXML.СвЮЛ.СведДолжнФЛ.СвДолжн?.НаимДолжн, НаимВидДолжн = documentXML.СвЮЛ.СведДолжнФЛ.СвДолжн?.НаимВидДолжн, ИННФЛ = documentXML.СвЮЛ.СведДолжнФЛ.СвФЛ?.ИННФЛ, ВидДолжн = documentXML.СвЮЛ.СведДолжнФЛ.СвДолжн?.ВидДолжн, idЛицо = ULDB.Id } };
                            //}

                            break;
                    }
                }

                //_dbcontext.ЮрЛицо.Update(ULDB);
                //try
                //{
                //    _dbcontext.SaveChanges();

                //}
                //catch (DbUpdateConcurrencyException ex)
                //{
                //    // если возникают ошибки параллельной обработки парсим документ еще раз
                //    if (documentXML.retry == false)
                //    {
                //        documentXML.retry = true;
                //        Console.WriteLine($"Возникла ошибка параллельной обработки лица {documentXML.СвЮЛ.ИНН}, совершается вторая попытка парсинга");
                //        if (entitiesInWork.Contains(documentXML.СвЮЛ.ИНН))
                //        {
                //            entitiesInWork.Remove(documentXML.СвЮЛ.ИНН);
                //        }
                //        ParseULDataDB((object)documentXML);
                //    }
                //}

                //try
                //{
                //    // после окончания работы убираем ЮЛ из списка обрабатываемых 
                //    if (entitiesInWork.Contains(documentXML.СвЮЛ.ИНН))
                //    {
                //        entitiesInWork.Remove(documentXML.СвЮЛ.ИНН);
                //    }
                //}
                //catch (Exception e)
                //{

                //}

                if (docCount - 1 == finishedWorkersCount)
                {
                    _dbcontext.ЕГРЮЛ_ОКВЭД.Where(e => ULSubTables.ЕГРЮЛ_ОКВЭД_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_ОКВЭД_Insert.DistinctBy(e => new { e.idЛицо, e.КодОквэд }).ToList());

                    _dbcontext.ЕГРЮЛ_СвАдресЮЛ.Where(e => ULSubTables.ЕГРЮЛ_СвАдресЮЛ_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвАдресЮЛ_Insert);

                    _dbcontext.ЕГРЮЛ_СвДержРеестрАО.Where(e => ULSubTables.ЕГРЮЛ_СвДержРеестрАО_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвДержРеестрАО_Insert);

                    _dbcontext.ЕГРЮЛ_СвДоляООО.Where(e => ULSubTables.ЕГРЮЛ_СвДоляООО_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвДоляООО_Insert.Select(e => e.Entry));

                    _dbcontext.ЕГРЮЛ_СвЗапЕГРЮЛ.Where(e => ULSubTables.ЕГРЮЛ_СвЗапЕГРЮЛ_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвЗапЕГРЮЛ_Insert);

                    _dbcontext.ЕГРЮЛ_СвНаимЮЛ.Where(e => ULSubTables.ЕГРЮЛ_СвНаимЮЛ_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвНаимЮЛ_Insert);

                    _dbcontext.ЕГРЮЛ_СвОбрЮЛ.Where(e => ULSubTables.ЕГРЮЛ_СвОбрЮЛ_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвОбрЮЛ_Insert);

                    _dbcontext.ЕГРЮЛ_СвПодразд.Where(e => ULSubTables.ЕГРЮЛ_СвПодразд_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвПодразд_Insert);

                    _dbcontext.ЕГРЮЛ_СвПредш.Where(e => ULSubTables.ЕГРЮЛ_СвПредш_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвПредш_Insert);

                    _dbcontext.ЕГРЮЛ_СвЛицензия.Where(e => ULSubTables.ЕГРЮЛ_СвЛицензия_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвЛицензия_Insert);

                    _dbcontext.ЕГРЮЛ_СвПреем.Where(e => ULSubTables.ЕГРЮЛ_СвПреем_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвПреем_Insert);

                    _dbcontext.ЕГРЮЛ_СвПрекрЮЛ.Where(e => ULSubTables.ЕГРЮЛ_СвПрекрЮЛ_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвПрекрЮЛ_Insert);

                    _dbcontext.ЕГРЮЛ_СвРегОрг.Where(e => ULSubTables.ЕГРЮЛ_СвРегОрг_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвРегОрг_Insert);

                    _dbcontext.ЕГРЮЛ_СвРегПФ.Where(e => ULSubTables.ЕГРЮЛ_СвРегПФ_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвРегПФ_Insert);

                    _dbcontext.ЕГРЮЛ_СвРегФСС.Where(e => ULSubTables.ЕГРЮЛ_СвРегФСС_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвРегФСС_Insert);

                    _dbcontext.ЕГРЮЛ_СвРеорг.Where(e => ULSubTables.ЕГРЮЛ_СвРеорг_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвРеорг_Insert);

                    _dbcontext.ЕГРЮЛ_СвСтатус.Where(e => ULSubTables.ЕГРЮЛ_СвСтатус_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвСтатус_Insert);

                    _dbcontext.ЕГРЮЛ_СвУстКап.Where(e => ULSubTables.ЕГРЮЛ_СвУстКап_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвУстКап_Insert);

                    _dbcontext.ЕГРЮЛ_СвУчетНО.Where(e => ULSubTables.ЕГРЮЛ_СвУчетНО_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвУчетНО_Insert);

                    _dbcontext.ЕГРЮЛ_СвУчредит.Where(e => ULSubTables.ЕГРЮЛ_СвУчредит_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвУчредит_Insert);

                    _dbcontext.ЕГРЮЛ_СвЮЛ.Where(e => ULSubTables.ЕГРЮЛ_СвЮЛ_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СвЮЛ_Insert);

                    _dbcontext.ЕГРЮЛ_СведДолжнФЛ.Where(e => ULSubTables.ЕГРЮЛ_СведДолжнФЛ_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.ЕГРЮЛ_СведДолжнФЛ_Insert);

                    _dbcontext.BulkCopy(IPSubTables.ИсторияИзменений_Insert);

                    ULSubTables.ClearULSubTables();
                }

                lock (locker)
                {
                    finishedWorkersCount++;
                }
            }

        }

        public async void ParseIPDataDB(object documentObject)
        {
            Документ documentXML = (Документ)documentObject;
            bool firstTime = false;

            if (documentXML.СвИП == null)
            {
                return;
            }

            //while (entitiesInWork.Contains(documentXML.СвИП.ИННФЛ))
            //{
            //    Console.WriteLine($"Лицо {documentXML.СвИП.ИННФЛ} уже в обработке, ожидание завершения.");
            //    Thread.Sleep(5000);
            //}

            //entitiesInWork.Add(documentXML.СвИП.ИННФЛ);

            using (DbContextTable _dbcontext = _contextFactory.CreateDbContext())
            {

                IP IPDB = new IP();

                IP data = (from u in _dbcontext.ИП where u.ИНН == documentXML.СвИП.ИННФЛ select u).FirstOrDefault();
                if (data != null)
                {
                    IPDB = data;
                }
                else
                {
                    lock (locker)
                    {
                        data = IPSubTables.IPInProcessing.Where(e => e.ИНН == documentXML.СвИП.ИННФЛ).FirstOrDefault();
                    }

                    if (data != null)
                    {
                        IPDB = data;
                    }
                    else
                    {
                        firstTime = true;
                        IPDB = new IP { ДатаОГРНИП = documentXML.СвИП.ДатаОГРНИП, ИНН = documentXML.СвИП.ИННФЛ, НаимВидИП = documentXML.СвИП.НаимВидИП, КодВидИП = documentXML.СвИП.КодВидИП, ОГРНИП = documentXML.СвИП.ОГРНИП, ИдДок = documentXML.ИдДок };
                        _dbcontext.ИП.Add(IPDB);
                        _dbcontext.SaveChanges();

                        lock (locker)
                        {
                            IPSubTables.IPInProcessing.Add(IPDB);
                        }
                    }
                }

                Document documentDB = new Document();
                documentDB.ДатаЗагрузки = DateTime.Now;
                documentDB.ИдДок = documentXML.ИдДок;
                documentDB.idИП = IPDB.Id;

                IPDB.document = new List<Document> { documentDB };
                _dbcontext.SaveChanges();

                _dbcontext.ИП.Update(IPDB);

                foreach (var entity in _dbcontext.Model.GetEntityTypes())
                {
                    switch (entity.ShortName())
                    {
                        case "EGRIPSVFL":
                            if (documentXML.СвИП.СвФЛ == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSVFL previousEntry;
                                var newEntry = new EGRIPSVFL { Id = Guid.NewGuid().ToString(), Пол = documentXML.СвИП.СвФЛ?.Пол, Отчество = documentXML.СвИП.СвФЛ?.ФИОРус?.Отчество, Имя = documentXML.СвИП.СвФЛ?.ФИОРус?.Имя, Фамилия = documentXML.СвИП.СвФЛ?.ФИОРус?.Фамилия, ДатаЗаписи = documentXML.СвИП.СвФЛ?.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвФЛ?.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id };

                                lock (listLockers[0])
                                {
                                    previousEntry = IPSubTables.ЕГРИП_СвФЛ_Insert.Where(e => e.idЛицо == IPDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[0])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвИП.СвФЛ.ГРНИПДата.ДатаЗаписи)
                                        {
                                            IPSubTables.ЕГРИП_СвФЛ_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            IPSubTables.ЕГРИП_СвФЛ_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвФЛ).Load();

                                        if (IPDB.ЕГРИП_СвФЛ?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.ЕГРИП_СвФЛ?.FirstOrDefault();
                                            IPSubTables.ЕГРИП_СвФЛ_Delete.Add(IPDB.ЕГРИП_СвФЛ?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[0])
                                    {
                                        IPSubTables.ЕГРИП_СвФЛ_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвИП.СвФЛ.ГРНИПДата.ДатаЗаписи)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSVFL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                Таблица = "EGRIPSVFL",
                                                Столбец = property.Name,
                                                ЗначениеДо = property.GetValue(previousEntry)?.ToString(),
                                                ЗначениеПосле = property.GetValue(newEntry)?.ToString(),
                                                ИНН = documentXML.СвИП.ИННФЛ,
                                                ДатаИзменения = DateTime.Now
                                            };

                                            // //_dbcontext.ИсторияИзменений.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                                //else
                                //{
                                //    lock (listLockers[0])
                                //    {
                                //        IPSubTables.ЕГРИП_СвФЛ_Insert.Add(new EGRIPSVFL { Id = IPDB.ЕГРИП_СвФЛ?.FirstOrDefault()?.Id != null ? IPDB.ЕГРИП_СвФЛ?.FirstOrDefault().Id : Guid.NewGuid().ToString(), Пол = documentXML.СвИП.СвФЛ?.Пол, Отчество = documentXML.СвИП.СвФЛ?.ФИОРус?.Отчество, Имя = documentXML.СвИП.СвФЛ?.ФИОРус?.Имя, Фамилия = documentXML.СвИП.СвФЛ?.ФИОРус?.Фамилия, ДатаЗаписи = documentXML.СвИП.СвФЛ?.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвФЛ?.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id });
                                //    }
                                //}
                            }

                            break;

                        case "EGRIPOKVED":
                            if (documentXML.СвИП.СвОКВЭД == null)
                            {
                                continue;
                            }
                            else
                            {
                                List<EGRIPOKVED> previousEntries;
                                var currentEntries = new List<EGRIPOKVED>();
                                var currentEntry = new EGRIPOKVED { Id = Guid.NewGuid().ToString(), Версия = documentXML.СвИП.СвОКВЭД?.СвОКВЭДОсн?.ПрВерсОКВЭД, Наименование = documentXML.СвИП.СвОКВЭД?.СвОКВЭДОсн?.НаимОКВЭД, КодОквэд = documentXML.СвИП.СвОКВЭД?.СвОКВЭДОсн?.КодОКВЭД, ОснКод = true, ДатаГРНИП = documentXML.СвИП.СвОКВЭД?.СвОКВЭДОсн?.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвОКВЭД?.СвОКВЭДОсн?.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id };
                                DateTime? currentEntriesDate = documentXML.СвИП.СвОКВЭД.СвОКВЭДОсн != null ? documentXML.СвИП.СвОКВЭД.СвОКВЭДОсн?.ГРНИПДата.ДатаЗаписи : documentXML.СвИП.СвОКВЭД.СвОКВЭДДоп?.FirstOrDefault().ГРНИПДата.ДатаЗаписи;

                                lock (listLockers[1])
                                {
                                    previousEntries = IPSubTables.ЕГРИП_ОКВЭД_Insert.Where(e => e.idЛицо == IPDB.Id).ToList();
                                }
                                if (previousEntries.Count != 0)
                                {
                                    lock (listLockers[1])
                                    {
                                        if (previousEntries.FirstOrDefault()?.ДатаГРНИП <= currentEntriesDate)
                                        {
                                            IPSubTables.ЕГРИП_ОКВЭД_Insert.RemoveAll(e => e.idЛицо == IPDB.Id);
                                            IPSubTables.ЕГРИП_ОКВЭД_Insert.Add(currentEntry);

                                            foreach (СвОКВЭДДоп okvedDop in documentXML.СвИП.СвОКВЭД?.СвОКВЭДДоп)
                                            {
                                                currentEntry = new EGRIPOKVED { Id = Guid.NewGuid().ToString(), КодОквэд = okvedDop.КодОКВЭД, ОснКод = false, Наименование = okvedDop.НаимОКВЭД, Версия = okvedDop.ПрВерсОКВЭД, ГРНИП = okvedDop.ГРНИПДата.ГРНИП, ДатаГРНИП = okvedDop.ГРНИПДата.ДатаЗаписи, idЛицо = IPDB.Id };
                                                currentEntries.Add(currentEntry);
                                                lock (listLockers[1])
                                                {
                                                    IPSubTables.ЕГРИП_ОКВЭД_Insert.Add(currentEntry);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_ОКВЭД).Load();

                                        if (IPDB.ЕГРИП_ОКВЭД?.FirstOrDefault() != null)
                                        {
                                            previousEntries = IPDB.ЕГРИП_ОКВЭД.ToList();
                                            IPSubTables.ЕГРИП_ОКВЭД_Delete.Add(IPDB.ЕГРИП_ОКВЭД?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[1])
                                    {
                                        IPSubTables.ЕГРИП_ОКВЭД_Insert.Add(currentEntry);
                                    }

                                    foreach (СвОКВЭДДоп okvedDop in documentXML.СвИП.СвОКВЭД?.СвОКВЭДДоп)
                                    {
                                        currentEntry = new EGRIPOKVED { Id = Guid.NewGuid().ToString(), КодОквэд = okvedDop.КодОКВЭД, ОснКод = false, Наименование = okvedDop.НаимОКВЭД, Версия = okvedDop.ПрВерсОКВЭД, ГРНИП = okvedDop.ГРНИПДата.ГРНИП, ДатаГРНИП = okvedDop.ГРНИПДата.ДатаЗаписи, idЛицо = IPDB.Id };
                                        currentEntries.Add(currentEntry);
                                        lock (listLockers[1])
                                        {
                                            IPSubTables.ЕГРИП_ОКВЭД_Insert.Add(currentEntry);
                                        }
                                    }
                                }

                                if (previousEntries?.Count > 0 && previousEntries.FirstOrDefault()?.ДатаГРНИП <= currentEntriesDate)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var oldEntry in previousEntries)
                                    {
                                        var newEntry = currentEntries.Where(e => e.КодОквэд == oldEntry.КодОквэд || e.Наименование == oldEntry.Наименование).FirstOrDefault();

                                        if (newEntry == null)
                                        {
                                            continue;
                                        }

                                        foreach (var property in typeof(EGRIPOKVED).GetProperties())
                                        {
                                            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                            {
                                                if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ИП")
                                                    continue;

                                                ChangeLog changes = new ChangeLog();

                                                changes.Таблица = "EGRIPOKVED";
                                                changes.Столбец = property.Name;
                                                changes.ЗначениеДо = property.GetValue(oldEntry)?.ToString();
                                                changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                                changes.ИНН = documentXML.СвИП.ИННФЛ;
                                                changes.ДатаИзменения = DateTime.Now;

                                                //_dbcontext.ИсторияИзменений.Add(changes);
                                                changeList.Add(changes);
                                            }
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                                //else
                                //{
                                //    lock (listLockers[1])
                                //    {
                                //        IPSubTables.ЕГРИП_ОКВЭД_Insert.Add(new EGRIPOKVED { Id = Guid.NewGuid().ToString(), Версия = documentXML.СвИП.СвОКВЭД?.СвОКВЭДОсн?.ПрВерсОКВЭД, Наименование = documentXML.СвИП.СвОКВЭД?.СвОКВЭДОсн?.НаимОКВЭД, КодОквэд = documentXML.СвИП.СвОКВЭД?.СвОКВЭДОсн?.КодОКВЭД, ОснКод = true, ДатаГРНИП = documentXML.СвИП.СвОКВЭД?.СвОКВЭДОсн?.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвОКВЭД?.СвОКВЭДОсн?.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id });

                                //        foreach (СвОКВЭДДоп okvedDop in documentXML.СвИП.СвОКВЭД?.СвОКВЭДДоп)
                                //        {
                                //            IPSubTables.ЕГРИП_ОКВЭД_Insert.Add(new EGRIPOKVED { Id = Guid.NewGuid().ToString(), КодОквэд = okvedDop.КодОКВЭД, ОснКод = false, Наименование = okvedDop.НаимОКВЭД, Версия = okvedDop.ПрВерсОКВЭД, ГРНИП = okvedDop.ГРНИПДата.ГРНИП, ДатаГРНИП = okvedDop.ГРНИПДата.ДатаЗаписи, idЛицо = IPDB.Id });
                                //        }
                                //    }                                    
                                //}
                            }

                            break;

                        case "EGRIPSvAdrMJ":

                            if (documentXML.СвИП.СвАдрМЖ == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvAdrMJ previousEntry;
                                var newEntry = new EGRIPSvAdrMJ { Id = Guid.NewGuid().ToString(), ДатаЗаписи = documentXML.СвИП.СвАдрМЖ.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвАдрМЖ.ГРНИПДата.ГРНИП, КодРегион = documentXML.СвИП.СвАдрМЖ.АдресРФ?.КодРегион, НаимГород = documentXML.СвИП.СвАдрМЖ.АдресРФ?.Город?.НаимГород, НаимНаселПункт = documentXML.СвИП.СвАдрМЖ.АдресРФ?.НаселПункт?.НаимНаселПункт, НаимРайон = documentXML.СвИП.СвАдрМЖ.АдресРФ?.Район?.НаимРайон, НаимРегион = documentXML.СвИП.СвАдрМЖ.АдресРФ?.Регион?.НаимРегион, ТипГород = documentXML.СвИП.СвАдрМЖ.АдресРФ?.Город?.ТипГород, ТипНаселПункт = documentXML.СвИП.СвАдрМЖ.АдресРФ?.НаселПункт?.ТипНаселПункт, ТипРайон = documentXML.СвИП.СвАдрМЖ.АдресРФ?.Район?.ТипРайон, ТипРегион = documentXML.СвИП.СвАдрМЖ.АдресРФ?.Регион?.ТипРегион, idЛицо = IPDB.Id };

                                lock (listLockers[2])
                                {
                                    previousEntry = IPSubTables.ЕГРИП_СвАдрМЖ_Insert.Where(e => e.idЛицо == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[2])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвИП.СвАдрМЖ.ГРНИПДата.ДатаЗаписи)
                                        {
                                            IPSubTables.ЕГРИП_СвАдрМЖ_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            IPSubTables.ЕГРИП_СвАдрМЖ_Insert.Add(newEntry);
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвАдрМЖ).Load();

                                        if (IPDB.ЕГРИП_СвАдрМЖ?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.ЕГРИП_СвАдрМЖ?.FirstOrDefault();
                                            IPSubTables.ЕГРИП_СвАдрМЖ_Delete.Add(IPDB.ЕГРИП_СвАдрМЖ?.FirstOrDefault().idЛицо.ToString());
                                        }

                                    }

                                    lock (listLockers[2])
                                    {
                                        IPSubTables.ЕГРИП_СвАдрМЖ_Insert.Add(newEntry);
                                    }
                                }



                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвИП.СвАдрМЖ.ГРНИПДата.ДатаЗаписи)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvAdrMJ).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                Таблица = "EGRIPSvAdrMJ",
                                                Столбец = property.Name,
                                                ЗначениеДо = property.GetValue(previousEntry)?.ToString(),
                                                ЗначениеПосле = property.GetValue(newEntry)?.ToString(),
                                                ИНН = documentXML.СвИП.ИННФЛ,
                                                ДатаИзменения = DateTime.Now
                                            };

                                            // //_dbcontext.ИсторияИзменений.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }

                                //else
                                //{                               
                                //_dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвАдрМЖ).Load();

                                //if (IPDB.ЕГРИП_СвАдрМЖ.FirstOrDefault() != null)
                                //{
                                //    _dbcontext.Entry(IPDB.ЕГРИП_СвАдрМЖ.FirstOrDefault()).State = EntityState.Deleted;
                                //    var previousEntry = IPDB.ЕГРИП_СвАдрМЖ.FirstOrDefault();
                                //    IPDB.ЕГРИП_СвАдрМЖ = new List<EGRIPSvAdrMJ> {  };
                                //    _dbcontext.Entry(IPDB.ЕГРИП_СвАдрМЖ.FirstOrDefault()).State = EntityState.Added;

                                //    List<ChangeLog> changeList = new List<ChangeLog>();

                                //    foreach (var property in typeof(EGRIPSvAdrMJ).GetProperties())
                                //    {
                                //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.ЕГРИП_СвАдрМЖ.FirstOrDefault())?.ToString())
                                //        {
                                //            ChangeLog changes = new ChangeLog();

                                //            changes.Таблица = "EGRIPSvAdrMJ";
                                //            changes.Столбец = property.Name;
                                //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                //            changes.ЗначениеПосле = property.GetValue(IPDB.ЕГРИП_СвАдрМЖ.FirstOrDefault())?.ToString();
                                //            changes.ИНН = documentXML.СвИП.ИННФЛ;
                                //            changes.ДатаИзменения = DateTime.Now;

                                //            //_dbcontext.ИсторияИзменений.Add(changes);
                                //            changeList.Add(changes);
                                //        }
                                //    }

                                //    _dbcontext.ИсторияИзменений.AddRange(changeList);
                                //}
                                //else
                                //{
                                //    IPDB.ЕГРИП_СвАдрМЖ = new List<EGRIPSvAdrMJ> { new EGRIPSvAdrMJ { Id = Guid.NewGuid().ToString(), ДатаЗаписи = documentXML.СвИП.СвАдрМЖ.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвАдрМЖ.ГРНИПДата.ГРНИП, КодРегион = documentXML.СвИП.СвАдрМЖ.АдресРФ?.КодРегион, НаимГород = documentXML.СвИП.СвАдрМЖ.АдресРФ?.Город?.НаимГород, НаимНаселПункт = documentXML.СвИП.СвАдрМЖ.АдресРФ?.НаселПункт?.НаимНаселПункт, НаимРайон = documentXML.СвИП.СвАдрМЖ.АдресРФ?.Район?.НаимРайон, НаимРегион = documentXML.СвИП.СвАдрМЖ.АдресРФ?.Регион?.НаимРегион, ТипГород = documentXML.СвИП.СвАдрМЖ.АдресРФ?.Город?.ТипГород, ТипНаселПункт = documentXML.СвИП.СвАдрМЖ.АдресРФ?.НаселПункт?.ТипНаселПункт, ТипРайон = documentXML.СвИП.СвАдрМЖ.АдресРФ?.Район?.ТипРайон, ТипРегион = documentXML.СвИП.СвАдрМЖ.АдресРФ?.Регион?.ТипРегион, idЛицо = IPDB.Id } };
                                //}
                            }

                            break;

                        case "EGRIPSvGrajd":
                            if (documentXML.СвИП.СвГражд == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvGrajd previousEntry;
                                var newEntry = new EGRIPSvGrajd { Id = Guid.NewGuid().ToString(), ВидГражд = documentXML.СвИП.СвГражд.ВидГражд, ДатаЗаписи = documentXML.СвИП.СвГражд.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвГражд.ГРНИПДата.ГРНИП, НаимСтран = documentXML.СвИП.СвГражд.НаимСтран, ОКСМ = documentXML.СвИП.СвГражд.ОКСМ, idЛицо = IPDB.Id };

                                lock (listLockers[3])
                                {
                                    previousEntry = IPSubTables.ЕГРИП_СвГражд_Insert.Where(e => e.idЛицо == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[3])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвИП.СвГражд.ГРНИПДата.ДатаЗаписи)
                                        {
                                            IPSubTables.ЕГРИП_СвГражд_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            IPSubTables.ЕГРИП_СвГражд_Insert.Add(newEntry);
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвГражд).Load();

                                        if (IPDB.ЕГРИП_СвГражд?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.ЕГРИП_СвГражд?.FirstOrDefault();
                                            IPSubTables.ЕГРИП_СвГражд_Delete.Add(IPDB.ЕГРИП_СвГражд?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[3])
                                    {
                                        IPSubTables.ЕГРИП_СвГражд_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвИП.СвГражд.ГРНИПДата.ДатаЗаписи)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvGrajd).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                Таблица = "EGRIPSvGrajd",
                                                Столбец = property.Name,
                                                ЗначениеДо = property.GetValue(previousEntry)?.ToString(),
                                                ЗначениеПосле = property.GetValue(newEntry)?.ToString(),
                                                ИНН = documentXML.СвИП.ИННФЛ,
                                                ДатаИзменения = DateTime.Now
                                            };

                                            // //_dbcontext.ИсторияИзменений.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }


                                //_dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвГражд).Load();

                                //if (IPDB.ЕГРИП_СвГражд.FirstOrDefault() != null)
                                //{
                                //    _dbcontext.Entry(IPDB.ЕГРИП_СвГражд.FirstOrDefault()).State = EntityState.Deleted;
                                //    var previousEntry = IPDB.ЕГРИП_СвГражд.FirstOrDefault();
                                //    IPDB.ЕГРИП_СвГражд = new List<EGRIPSvGrajd> { new EGRIPSvGrajd { Id = IPDB.ЕГРИП_СвГражд.FirstOrDefault()?.Id != null ? IPDB.ЕГРИП_СвГражд.FirstOrDefault().Id : Guid.NewGuid().ToString(), ВидГражд = documentXML.СвИП.СвГражд.ВидГражд, ДатаЗаписи = documentXML.СвИП.СвГражд.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвГражд.ГРНИПДата.ГРНИП, НаимСтран = documentXML.СвИП.СвГражд.НаимСтран, ОКСМ = documentXML.СвИП.СвГражд.ОКСМ, idЛицо = IPDB.Id } };
                                //    _dbcontext.Entry(IPDB.ЕГРИП_СвГражд.FirstOrDefault()).State = EntityState.Added;

                                //    List<ChangeLog> changeList = new List<ChangeLog>();

                                //    foreach (var property in typeof(EGRIPSvGrajd).GetProperties())
                                //    {
                                //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.ЕГРИП_СвГражд.FirstOrDefault())?.ToString())
                                //        {
                                //            ChangeLog changes = new ChangeLog();

                                //            changes.Таблица = "EGRIPSvGrajd";
                                //            changes.Столбец = property.Name;
                                //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                                //            changes.ЗначениеПосле = property.GetValue(IPDB.ЕГРИП_СвГражд.FirstOrDefault())?.ToString();
                                //            changes.ИНН = documentXML.СвИП.ИННФЛ;
                                //            changes.ДатаИзменения = DateTime.Now;

                                //            //_dbcontext.ИсторияИзменений.Add(changes);
                                //            changeList.Add(changes);
                                //        }
                                //    }

                                //    _dbcontext.ИсторияИзменений.AddRange(changeList);
                                //}
                                //else
                                //{
                                //    IPDB.ЕГРИП_СвГражд = new List<EGRIPSvGrajd> { new EGRIPSvGrajd { Id = Guid.NewGuid().ToString(), ВидГражд = documentXML.СвИП.СвГражд.ВидГражд, ДатаЗаписи = documentXML.СвИП.СвГражд.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвГражд.ГРНИПДата.ГРНИП, НаимСтран = documentXML.СвИП.СвГражд.НаимСтран, ОКСМ = documentXML.СвИП.СвГражд.ОКСМ, idЛицо = IPDB.Id } };
                                //}
                            }

                            break;

                        case "EGRIPSvIP":
                            if (documentXML.СвИП == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvIP previousEntry;
                                var newEntry = new EGRIPSvIP { Id = Guid.NewGuid().ToString(), НаимВидИП = documentXML.СвИП.НаимВидИП, КодВидИП = documentXML.СвИП.КодВидИП, ИНН = documentXML.СвИП.ИННФЛ, ДатаОГРН = documentXML.СвИП.ДатаОГРНИП, ОГРН = documentXML.СвИП.ОГРНИП, ДатаВып = documentXML.СвИП.ДатаВып, НаимСокр = documentXML.СвИП.СвФЛ?.ФИОРус?.Фамилия + " " + documentXML.СвИП.СвФЛ?.ФИОРус?.Имя + " " + documentXML.СвИП.СвФЛ?.ФИОРус?.Отчество, ОКВЭДОсн = documentXML.СвИП.СвОКВЭД?.СвОКВЭДОсн?.КодОКВЭД, idЛицо = IPDB.Id };

                                lock (listLockers[4])
                                {
                                    previousEntry = IPSubTables.ЕГРИП_СвИП_Insert.Where(e => e.idЛицо == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[4])
                                    {
                                        if (previousEntry.ДатаОГРН <= documentXML.СвИП.ДатаОГРНИП)
                                        {
                                            IPSubTables.ЕГРИП_СвИП_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            IPSubTables.ЕГРИП_СвИП_Insert.Add(newEntry);
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвИП).Load();

                                        if (IPDB.ЕГРИП_СвИП?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.ЕГРИП_СвИП?.FirstOrDefault();
                                            IPSubTables.ЕГРИП_СвИП_Delete.Add(IPDB.ЕГРИП_СвИП?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[4])
                                    {
                                        IPSubTables.ЕГРИП_СвИП_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаОГРН <= documentXML.СвИП.ДатаОГРНИП)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvIP).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                Таблица = "EGRIPSvIP",
                                                Столбец = property.Name,
                                                ЗначениеДо = property.GetValue(previousEntry)?.ToString(),
                                                ЗначениеПосле = property.GetValue(newEntry)?.ToString(),
                                                ИНН = documentXML.СвИП.ИННФЛ,
                                                ДатаИзменения = DateTime.Now
                                            };

                                            // //_dbcontext.ИсторияИзменений.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвИП).Load();

                            //if (IPDB.ЕГРИП_СвИП.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.ЕГРИП_СвИП.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвИП.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.ЕГРИП_СвИП = new List<EGRIPSvIP> { new EGRIPSvIP { Id = IPDB.ЕГРИП_СвИП.FirstOrDefault()?.Id != null ? IPDB.ЕГРИП_СвИП.FirstOrDefault().Id : Guid.NewGuid().ToString(), НаимВидИП = documentXML.СвИП.НаимВидИП, КодВидИП = documentXML.СвИП.КодВидИП, ИНН = documentXML.СвИП.ИННФЛ, ДатаОГРН = documentXML.СвИП.ДатаОГРНИП, ОГРН = documentXML.СвИП.ОГРНИП, ДатаВып = documentXML.СвИП.ДатаВып, НаимСокр = documentXML.СвИП.СвФЛ?.ФИОРус?.Фамилия + " " + documentXML.СвИП.СвФЛ?.ФИОРус?.Имя + " " + documentXML.СвИП.СвФЛ?.ФИОРус?.Отчество, ОКВЭДОсн = documentXML.СвИП.СвОКВЭД?.СвОКВЭДОсн?.КодОКВЭД, idЛицо = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвИП.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvIP).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.ЕГРИП_СвИП.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRIPSvIP";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(IPDB.ЕГРИП_СвИП.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвИП.ИННФЛ;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.ИсторияИзменений.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.ЕГРИП_СвИП = new List<EGRIPSvIP> { new EGRIPSvIP { Id = Guid.NewGuid().ToString(), НаимВидИП = documentXML.СвИП.НаимВидИП, КодВидИП = documentXML.СвИП.КодВидИП, ИНН = documentXML.СвИП.ИННФЛ, ДатаОГРН = documentXML.СвИП.ДатаОГРНИП, ОГРН = documentXML.СвИП.ОГРНИП, ДатаВып = documentXML.СвИП.ДатаВып, НаимСокр = documentXML.СвИП.СвФЛ?.ФИОРус?.Фамилия + " " + documentXML.СвИП.СвФЛ?.ФИОРус?.Имя + " " + documentXML.СвИП.СвФЛ?.ФИОРус?.Отчество, ОКВЭДОсн = documentXML.СвИП.СвОКВЭД?.СвОКВЭДОсн?.КодОКВЭД, idЛицо = IPDB.Id } };
                            //}

                            break;

                        case "EGRIPSvLicense":
                            if (documentXML.СвИП.СвЛицензия == null)
                            {
                                continue;
                            }
                            else
                            {
                                List<EGRIPSvLicense> previousEntries = new List<EGRIPSvLicense>();
                                var currentEntries = new List<EGRIPSvLicense>();
                                var currentEntry = new EGRIPSvLicense();
                                DateTime previousEntriesDate;

                                lock (listLockers[5])
                                {
                                    foreach (var entryDatePair in IPSubTables.ЕГРИП_СвЛицензия_Insert.Where(e => e.Entry.idЛицо == IPDB.Id))
                                    {
                                        previousEntries.Add(entryDatePair.Entry);
                                    }

                                    var licenseArray = IPSubTables.ЕГРИП_СвЛицензия_Insert.Where(e => e.Entry.idЛицо == IPDB.Id).Select(e => e.Date).ToArray();
                                    previousEntriesDate = licenseArray.Count() > 0 ? licenseArray.Max() : DateTime.MinValue;
                                }
                                if (previousEntries.Count != 0)
                                {
                                    lock (listLockers[5])
                                    {
                                        if (previousEntriesDate <= documentXML.СвИП.ДатаВып)
                                        {
                                            IPSubTables.ЕГРИП_СвЛицензия_Insert.RemoveAll(e => e.Entry.idЛицо == IPDB.Id);

                                            foreach (var svLicense in documentXML.СвИП.СвЛицензия)
                                            {
                                                currentEntry = new EGRIPSvLicense { Id = Guid.NewGuid().ToString(), ДатаНачЛиц = svLicense.ДатаНачЛиц, ДатаЛиц = svLicense.ДатаЛиц, НомЛиц = svLicense.НомЛиц, НаимЛицВидДеят = svLicense.НаимЛицВидДеят, ЛицОргВыдЛиц = svLicense.ЛицОргВыдЛиц, ДатаОкончЛиц = svLicense.ДатаОкончЛиц, ДатаЗаписи = svLicense.ГРНИПДата.ДатаЗаписи, ГРНИП = svLicense.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id };
                                                currentEntries.Add(currentEntry);
                                                IPSubTables.ЕГРИП_СвЛицензия_Insert.Add((currentEntry, documentXML.СвИП.ДатаВып));
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвЛицензия).Load();
                                        if (IPDB.ЕГРИП_СвЛицензия?.FirstOrDefault() != null)
                                        {
                                            previousEntries = IPDB.ЕГРИП_СвЛицензия.ToList();
                                            IPSubTables.ЕГРИП_СвЛицензия_Delete.Add(IPDB.ЕГРИП_СвЛицензия?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }
                                    lock (listLockers[5])
                                    {
                                        foreach (var svLicense in documentXML.СвИП.СвЛицензия)
                                        {
                                            currentEntry = new EGRIPSvLicense { Id = Guid.NewGuid().ToString(), ДатаНачЛиц = svLicense.ДатаНачЛиц, ДатаЛиц = svLicense.ДатаЛиц, НомЛиц = svLicense.НомЛиц, НаимЛицВидДеят = svLicense.НаимЛицВидДеят, ЛицОргВыдЛиц = svLicense.ЛицОргВыдЛиц, ДатаОкончЛиц = svLicense.ДатаОкончЛиц, ДатаЗаписи = svLicense.ГРНИПДата.ДатаЗаписи, ГРНИП = svLicense.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id };
                                            currentEntries.Add(currentEntry);
                                            IPSubTables.ЕГРИП_СвЛицензия_Insert.Add((currentEntry, documentXML.СвИП.ДатаВып));
                                        }
                                    }
                                }
                                if (previousEntries?.Count > 0 && previousEntriesDate <= documentXML.СвИП.ДатаВып)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();
                                    foreach (var oldEntry in previousEntries)
                                    {
                                        var newEntry = currentEntries.Where(e => e.НомЛиц == oldEntry.НомЛиц).FirstOrDefault();
                                        if (newEntry == null)
                                        {
                                            continue;
                                        }
                                        foreach (var property in typeof(EGRIPSvLicense).GetProperties())
                                        {

                                            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                            {
                                                if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ИП")
                                                    continue;

                                                ChangeLog changes = new ChangeLog();

                                                changes.Таблица = "EGRIPSvLicense";
                                                changes.Столбец = property.Name;
                                                changes.ЗначениеДо = property.GetValue(oldEntry)?.ToString();
                                                changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                                                changes.ИНН = documentXML.СвИП.ИННФЛ;
                                                changes.ДатаИзменения = DateTime.Now;

                                                //_dbcontext.ИсторияИзменений.Add(changes);
                                                changeList.Add(changes);
                                            }
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }

                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвЛицензия).Load();

                            //if (IPDB.ЕГРИП_СвЛицензия.Count > 0)
                            //{
                            //    foreach (EGRIPSvLicense entry in IPDB.ЕГРИП_СвЛицензия)
                            //    {
                            //        _dbcontext.Entry(entry).State = EntityState.Deleted;
                            //    }

                            //    var previousEntries = IPDB.ЕГРИП_СвЛицензия;
                            //    IPDB.ЕГРИП_СвЛицензия = new List<EGRIPSvLicense>();

                            //    foreach (var svLicense in documentXML.СвИП.СвЛицензия)
                            //    {
                            //        IPDB.ЕГРИП_СвЛицензия.Add(new EGRIPSvLicense { Id = Guid.NewGuid().ToString(), ДатаНачЛиц = svLicense.ДатаНачЛиц, ДатаЛиц = svLicense.ДатаЛиц, НомЛиц = svLicense.НомЛиц, НаимЛицВидДеят = svLicense.НаимЛицВидДеят, ЛицОргВыдЛиц = svLicense.ЛицОргВыдЛиц, ДатаОкончЛиц = svLicense.ДатаОкончЛиц, ДатаЗаписи = svLicense.ГРНИПДата.ДатаЗаписи, ГРНИП = svLicense.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id });
                            //        _dbcontext.Entry(IPDB.ЕГРИП_СвЛицензия.Last()).State = EntityState.Added;
                            //    }

                            //    var currentEntries = IPDB.ЕГРИП_СвЛицензия;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var oldEntry in previousEntries)
                            //    {
                            //        var newEntry = currentEntries.Where(e => e.НомЛиц == oldEntry.НомЛиц).FirstOrDefault();

                            //        if (newEntry == null)
                            //        {
                            //            continue;
                            //        }

                            //        foreach (var property in typeof(EGRIPSvLicense).GetProperties())
                            //        {

                            //            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                            //            {
                            //                if (property.Name == "Id" || property.Name == "idЛицо" || property.Name == "ИП")
                            //                    continue;

                            //                ChangeLog changes = new ChangeLog();

                            //                changes.Таблица = "EGRIPSvLicense";
                            //                changes.Столбец = property.Name;
                            //                changes.ЗначениеДо = property.GetValue(oldEntry)?.ToString();
                            //                changes.ЗначениеПосле = property.GetValue(newEntry)?.ToString();
                            //                changes.ИНН = documentXML.СвИП.ИННФЛ;
                            //                changes.ДатаИзменения = DateTime.Now;

                            //                //_dbcontext.ИсторияИзменений.Add(changes);
                            //                changeList.Add(changes);
                            //            }
                            //        }
                            //    }

                            //    _dbcontext.ИсторияИзменений.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.ЕГРИП_СвЛицензия = new List<EGRIPSvLicense>();

                            //    foreach (var svLicense in documentXML.СвИП.СвЛицензия)
                            //    {
                            //        IPDB.ЕГРИП_СвЛицензия.Add(new EGRIPSvLicense { Id = Guid.NewGuid().ToString(), ДатаНачЛиц = svLicense.ДатаНачЛиц, ДатаЛиц = svLicense.ДатаЛиц, НомЛиц = svLicense.НомЛиц, НаимЛицВидДеят = svLicense.НаимЛицВидДеят, ЛицОргВыдЛиц = svLicense.ЛицОргВыдЛиц, ДатаОкончЛиц = svLicense.ДатаОкончЛиц, ДатаЗаписи = svLicense.ГРНИПДата.ДатаЗаписи, ГРНИП = svLicense.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id });
                            //    }
                            //}

                            break;

                        case "EGRIPSvPrekras_":
                            if (documentXML.СвИП.СвПрекращ == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvPrekras_ previousEntry;
                                var newEntry = new EGRIPSvPrekras_ { Id = Guid.NewGuid().ToString(), ДатаПрекращ = documentXML.СвИП.СвПрекращ.СвСтатус?.ДатаПрекращ, КодСтатус = documentXML.СвИП.СвПрекращ.СвСтатус?.КодСтатус, НаимСтатус = documentXML.СвИП.СвПрекращ.СвСтатус?.НаимСтатус, ДатаЗаписи = documentXML.СвИП.СвПрекращ.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвПрекращ.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id };

                                lock (listLockers[6])
                                {
                                    previousEntry = IPSubTables.ЕГРИП_СвПрекращ_Insert.Where(e => e.idЛицо == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[6])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвИП.СвПрекращ.ГРНИПДата.ДатаЗаписи)
                                        {
                                            IPSubTables.ЕГРИП_СвПрекращ_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            IPSubTables.ЕГРИП_СвПрекращ_Insert.Add(newEntry);
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвПрекращ).Load();

                                        if (IPDB.ЕГРИП_СвПрекращ?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.ЕГРИП_СвПрекращ?.FirstOrDefault();
                                            IPSubTables.ЕГРИП_СвПрекращ_Delete.Add(IPDB.ЕГРИП_СвПрекращ?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[6])
                                    {
                                        IPSubTables.ЕГРИП_СвПрекращ_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвИП.СвПрекращ.ГРНИПДата.ДатаЗаписи)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvPrekras_).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                Таблица = "EGRIPSvPrekras_",
                                                Столбец = property.Name,
                                                ЗначениеДо = property.GetValue(previousEntry)?.ToString(),
                                                ЗначениеПосле = property.GetValue(newEntry)?.ToString(),
                                                ИНН = documentXML.СвИП.ИННФЛ,
                                                ДатаИзменения = DateTime.Now
                                            };

                                            // //_dbcontext.ИсторияИзменений.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвПрекращ).Load();

                            //if (IPDB.ЕГРИП_СвПрекращ.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.ЕГРИП_СвПрекращ.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвПрекращ.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.ЕГРИП_СвПрекращ = new List<EGRIPSvPrekras_> { new EGRIPSvPrekras_ { Id = IPDB.ЕГРИП_СвПрекращ.FirstOrDefault()?.Id != null ? IPDB.ЕГРИП_СвПрекращ.FirstOrDefault().Id : Guid.NewGuid().ToString(), ДатаПрекращ = documentXML.СвИП.СвПрекращ.СвСтатус?.ДатаПрекращ, КодСтатус = documentXML.СвИП.СвПрекращ.СвСтатус?.КодСтатус, НаимСтатус = documentXML.СвИП.СвПрекращ.СвСтатус?.НаимСтатус, ДатаЗаписи = documentXML.СвИП.СвПрекращ.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвПрекращ.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвПрекращ.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvPrekras_).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.ЕГРИП_СвПрекращ.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRIPSvPrekras_";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(IPDB.ЕГРИП_СвПрекращ.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвИП.ИННФЛ;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.ИсторияИзменений.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.ЕГРИП_СвПрекращ = new List<EGRIPSvPrekras_> { new EGRIPSvPrekras_ { Id = Guid.NewGuid().ToString(), ДатаПрекращ = documentXML.СвИП.СвПрекращ.СвСтатус?.ДатаПрекращ, КодСтатус = documentXML.СвИП.СвПрекращ.СвСтатус?.КодСтатус, НаимСтатус = documentXML.СвИП.СвПрекращ.СвСтатус?.НаимСтатус, ДатаЗаписи = documentXML.СвИП.СвПрекращ.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвПрекращ.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id } };
                            //}

                            break;

                        case "EGRIPSvRegIP":
                            if (documentXML.СвИП.СвРегИП == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvRegIP previousEntry;
                                var newEntry = new EGRIPSvRegIP { Id = Guid.NewGuid().ToString(), ДатаЗаписи = documentXML.СвИП.СвРегИП.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвРегИП.ГРНИП, ДатаОГРНИП = documentXML.СвИП.СвРегИП.ДатаОГРНИП, ДатаРег = documentXML.СвИП.СвРегИП.ДатаРег, ИНН = documentXML.СвИП.СвРегИП.ИНН, НаимРО = documentXML.СвИП.СвРегИП.НаимРО, НаимЮЛПолн = documentXML.СвИП.СвРегИП.НаимЮЛПолн, ОГРН = documentXML.СвИП.СвРегИП.ОГРН, ОГРНИП = documentXML.СвИП.СвРегИП.ОГРНИП, РегНом = documentXML.СвИП.СвРегИП.РегНом, idЛицо = IPDB.Id };

                                lock (listLockers[7])
                                {
                                    previousEntry = IPSubTables.ЕГРИП_СвРегИП_Insert.Where(e => e.idЛицо == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[7])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвИП.СвРегИП.ДатаЗаписи)
                                        {
                                            IPSubTables.ЕГРИП_СвРегИП_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            IPSubTables.ЕГРИП_СвРегИП_Insert.Add(newEntry);
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвРегИП).Load();

                                        if (IPDB.ЕГРИП_СвРегИП?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.ЕГРИП_СвРегИП?.FirstOrDefault();
                                            IPSubTables.ЕГРИП_СвРегИП_Delete.Add(IPDB.ЕГРИП_СвРегИП?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }
                                    lock (listLockers[7])
                                    {
                                        IPSubTables.ЕГРИП_СвРегИП_Insert.Add(newEntry);
                                    }
                                }



                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвИП.СвРегИП.ДатаЗаписи)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvRegIP).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                Таблица = "EGRIPSvRegIP",
                                                Столбец = property.Name,
                                                ЗначениеДо = property.GetValue(previousEntry)?.ToString(),
                                                ЗначениеПосле = property.GetValue(newEntry)?.ToString(),
                                                ИНН = documentXML.СвИП.ИННФЛ,
                                                ДатаИзменения = DateTime.Now
                                            };

                                            // //_dbcontext.ИсторияИзменений.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвРегИП).Load();

                            //if (IPDB.ЕГРИП_СвРегИП.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.ЕГРИП_СвРегИП.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвРегИП.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.ЕГРИП_СвРегИП = new List<EGRIPSvRegIP> { new EGRIPSvRegIP { Id = IPDB.ЕГРИП_СвРегИП.FirstOrDefault()?.Id != null ? IPDB.ЕГРИП_СвРегИП.FirstOrDefault().Id : Guid.NewGuid().ToString(), ДатаЗаписи = documentXML.СвИП.СвРегИП.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвРегИП.ГРНИП, ДатаОГРНИП = documentXML.СвИП.СвРегИП.ДатаОГРНИП, ДатаРег = documentXML.СвИП.СвРегИП.ДатаРег, ИНН = documentXML.СвИП.СвРегИП.ИНН, НаимРО = documentXML.СвИП.СвРегИП.НаимРО, НаимЮЛПолн = documentXML.СвИП.СвРегИП.НаимЮЛПолн, ОГРН = documentXML.СвИП.СвРегИП.ОГРН, ОГРНИП = documentXML.СвИП.СвРегИП.ОГРНИП, РегНом = documentXML.СвИП.СвРегИП.РегНом, idЛицо = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвРегИП.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvRegIP).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.ЕГРИП_СвРегИП.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRIPSvRegIP";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(IPDB.ЕГРИП_СвРегИП.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвИП.ИННФЛ;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.ИсторияИзменений.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.ЕГРИП_СвРегИП = new List<EGRIPSvRegIP> { new EGRIPSvRegIP { Id = Guid.NewGuid().ToString(), ДатаЗаписи = documentXML.СвИП.СвРегИП.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвРегИП.ГРНИП, ДатаОГРНИП = documentXML.СвИП.СвРегИП.ДатаОГРНИП, ДатаРег = documentXML.СвИП.СвРегИП.ДатаРег, ИНН = documentXML.СвИП.СвРегИП.ИНН, НаимРО = documentXML.СвИП.СвРегИП.НаимРО, НаимЮЛПолн = documentXML.СвИП.СвРегИП.НаимЮЛПолн, ОГРН = documentXML.СвИП.СвРегИП.ОГРН, ОГРНИП = documentXML.СвИП.СвРегИП.ОГРНИП, РегНом = documentXML.СвИП.СвРегИП.РегНом, idЛицо = IPDB.Id } };
                            //}

                            break;

                        case "EGRIPSvGegOrg":
                            if (documentXML.СвИП.СвРегОрг == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvGegOrg previousEntry;
                                var newEntry = new EGRIPSvGegOrg { Id = Guid.NewGuid().ToString(), АдрРО = documentXML.СвИП.СвРегОрг?.АдрРО, КодНО = documentXML.СвИП.СвРегОрг?.КодНО, НаимНО = documentXML.СвИП.СвРегОрг?.НаимНО, ДатаЗаписи = documentXML.СвИП.СвРегОрг?.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвРегОрг?.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id };

                                lock (listLockers[8])
                                {
                                    previousEntry = IPSubTables.ЕГРИП_СвРегОрг_Insert.Where(e => e.idЛицо == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[8])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвИП.СвРегОрг.ГРНИПДата.ДатаЗаписи)
                                        {
                                            IPSubTables.ЕГРИП_СвРегОрг_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            IPSubTables.ЕГРИП_СвРегОрг_Insert.Add(newEntry);
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвРегОрг).Load();

                                        if (IPDB.ЕГРИП_СвРегОрг?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.ЕГРИП_СвРегОрг?.FirstOrDefault();
                                            IPSubTables.ЕГРИП_СвРегОрг_Delete.Add(IPDB.ЕГРИП_СвРегОрг?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }
                                    lock (listLockers[8])
                                    {
                                        IPSubTables.ЕГРИП_СвРегОрг_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвИП.СвРегОрг.ГРНИПДата.ДатаЗаписи)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvGegOrg).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                Таблица = "EGRIPSvGegOrg",
                                                Столбец = property.Name,
                                                ЗначениеДо = property.GetValue(previousEntry)?.ToString(),
                                                ЗначениеПосле = property.GetValue(newEntry)?.ToString(),
                                                ИНН = documentXML.СвИП.ИННФЛ,
                                                ДатаИзменения = DateTime.Now
                                            };

                                            // //_dbcontext.ИсторияИзменений.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвРегОрг).Load();

                            //if (IPDB.ЕГРИП_СвРегОрг?.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.ЕГРИП_СвРегОрг?.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвРегОрг?.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.ЕГРИП_СвРегОрг = new List<EGRIPSvGegOrg> { new EGRIPSvGegOrg { Id = IPDB.ЕГРИП_СвРегОрг?.FirstOrDefault()?.Id != null ? IPDB.ЕГРИП_СвРегОрг?.FirstOrDefault().Id : Guid.NewGuid().ToString(), АдрРО = documentXML.СвИП.СвРегОрг?.АдрРО, КодНО = documentXML.СвИП.СвРегОрг?.КодНО, НаимНО = documentXML.СвИП.СвРегОрг?.НаимНО, ДатаЗаписи = documentXML.СвИП.СвРегОрг?.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвРегОрг?.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвРегОрг?.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvGegOrg).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.ЕГРИП_СвРегОрг?.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRIPSvGegOrg";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(IPDB.ЕГРИП_СвРегОрг?.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвИП.ИННФЛ;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.ИсторияИзменений.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.ЕГРИП_СвРегОрг = new List<EGRIPSvGegOrg> { new EGRIPSvGegOrg { Id = Guid.NewGuid().ToString(), АдрРО = documentXML.СвИП.СвРегОрг?.АдрРО, КодНО = documentXML.СвИП.СвРегОрг?.КодНО, НаимНО = documentXML.СвИП.СвРегОрг?.НаимНО, ДатаЗаписи = documentXML.СвИП.СвРегОрг?.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвРегОрг?.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id } };
                            //}

                            break;

                        case "EGRIPSvRegPF":
                            if (documentXML.СвИП.СвРегПФ == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvRegPF previousEntry;
                                var newEntry = new EGRIPSvRegPF { Id = Guid.NewGuid().ToString(), ДатаРег = documentXML.СвИП.СвРегПФ.ДатаРег, РегНомПФ = documentXML.СвИП.СвРегПФ.РегНомПФ, НаимПФ = documentXML.СвИП.СвРегПФ.СвОргПФ?.НаимПФ, КодПФ = documentXML.СвИП.СвРегПФ.СвОргПФ?.КодПФ, ДатаЗаписи = documentXML.СвИП.СвРегПФ.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвРегПФ.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id };

                                lock (listLockers[9])
                                {
                                    previousEntry = IPSubTables.ЕГРИП_СвРегПФ_Insert.Where(e => e.idЛицо == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[9])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвИП.СвРегПФ.ГРНИПДата.ДатаЗаписи)
                                        {
                                            IPSubTables.ЕГРИП_СвРегПФ_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            IPSubTables.ЕГРИП_СвРегПФ_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвРегПФ).Load();

                                        if (IPDB.ЕГРИП_СвРегПФ?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.ЕГРИП_СвРегПФ?.FirstOrDefault();
                                            IPSubTables.ЕГРИП_СвРегПФ_Delete.Add(IPDB.ЕГРИП_СвРегПФ?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }
                                    lock (listLockers[9])
                                    {
                                        IPSubTables.ЕГРИП_СвРегПФ_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвИП.СвРегПФ.ГРНИПДата.ДатаЗаписи)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvRegPF).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                Таблица = "EGRIPSvRegPF",
                                                Столбец = property.Name,
                                                ЗначениеДо = property.GetValue(previousEntry)?.ToString(),
                                                ЗначениеПосле = property.GetValue(newEntry)?.ToString(),
                                                ИНН = documentXML.СвИП.ИННФЛ,
                                                ДатаИзменения = DateTime.Now
                                            };

                                            // //_dbcontext.ИсторияИзменений.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвРегПФ).Load();

                            //if (IPDB.ЕГРИП_СвРегПФ.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.ЕГРИП_СвРегПФ.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвРегПФ.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.ЕГРИП_СвРегПФ = new List<EGRIPSvRegPF> { new EGRIPSvRegPF { Id = IPDB.ЕГРИП_СвРегПФ.FirstOrDefault()?.Id != null ? IPDB.ЕГРИП_СвРегПФ.FirstOrDefault().Id : Guid.NewGuid().ToString(), ДатаРег = documentXML.СвИП.СвРегПФ.ДатаРег, РегНомПФ = documentXML.СвИП.СвРегПФ.РегНомПФ, НаимПФ = documentXML.СвИП.СвРегПФ.СвОргПФ?.НаимПФ, КодПФ = documentXML.СвИП.СвРегПФ.СвОргПФ?.КодПФ, ДатаЗаписи = documentXML.СвИП.СвРегПФ.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвРегПФ.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвРегПФ.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvRegPF).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.ЕГРИП_СвРегПФ.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRIPSvRegPF";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(IPDB.ЕГРИП_СвРегПФ.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвИП.ИННФЛ;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.ИсторияИзменений.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.ЕГРИП_СвРегПФ = new List<EGRIPSvRegPF> { new EGRIPSvRegPF { Id = Guid.NewGuid().ToString(), ДатаРег = documentXML.СвИП.СвРегПФ.ДатаРег, РегНомПФ = documentXML.СвИП.СвРегПФ.РегНомПФ, НаимПФ = documentXML.СвИП.СвРегПФ.СвОргПФ?.НаимПФ, КодПФ = documentXML.СвИП.СвРегПФ.СвОргПФ?.КодПФ, ДатаЗаписи = documentXML.СвИП.СвРегПФ.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвРегПФ.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id } };
                            //}

                            break;

                        case "EGRIPSvRegFSS":
                            if (documentXML.СвИП.СвРегФСС == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvRegFSS previousEntry;
                                var newEntry = new EGRIPSvRegFSS { Id = Guid.NewGuid().ToString(), ДатаРег = documentXML.СвИП.СвРегФСС.ДатаРег, РегНомФСС = documentXML.СвИП.СвРегФСС.РегНомФСС, НаимФСС = documentXML.СвИП.СвРегФСС.СвОргФСС?.НаимФСС, КодФСС = documentXML.СвИП.СвРегФСС.СвОргФСС?.КодФСС, ДатаЗаписи = documentXML.СвИП.СвРегФСС.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвРегФСС.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id };

                                lock (listLockers[10])
                                {
                                    previousEntry = IPSubTables.ЕГРИП_СвРегФСС_Insert.Where(e => e.idЛицо == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[10])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвИП.СвРегФСС.ГРНИПДата.ДатаЗаписи)
                                        {
                                            IPSubTables.ЕГРИП_СвРегФСС_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            IPSubTables.ЕГРИП_СвРегФСС_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвРегФСС).Load();

                                        if (IPDB.ЕГРИП_СвРегФСС?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.ЕГРИП_СвРегФСС?.FirstOrDefault();
                                            IPSubTables.ЕГРИП_СвРегФСС_Delete.Add(IPDB.ЕГРИП_СвРегФСС?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }
                                    lock (listLockers[10])
                                    {
                                        IPSubTables.ЕГРИП_СвРегФСС_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвИП.СвРегФСС.ГРНИПДата.ДатаЗаписи)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvRegFSS).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                Таблица = "EGRIPSvRegFSS",
                                                Столбец = property.Name,
                                                ЗначениеДо = property.GetValue(previousEntry)?.ToString(),
                                                ЗначениеПосле = property.GetValue(newEntry)?.ToString(),
                                                ИНН = documentXML.СвИП.ИННФЛ,
                                                ДатаИзменения = DateTime.Now
                                            };

                                            // //_dbcontext.ИсторияИзменений.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвРегФСС).Load();

                            //if (IPDB.ЕГРИП_СвРегФСС.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.ЕГРИП_СвРегФСС.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвРегФСС.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.ЕГРИП_СвРегФСС = new List<EGRIPSvRegFSS> { new EGRIPSvRegFSS { Id = IPDB.ЕГРИП_СвРегФСС.FirstOrDefault()?.Id != null ? IPDB.ЕГРИП_СвРегФСС.FirstOrDefault().Id : Guid.NewGuid().ToString(), ДатаРег = documentXML.СвИП.СвРегФСС.ДатаРег, РегНомФСС = documentXML.СвИП.СвРегФСС.РегНомФСС, НаимФСС = documentXML.СвИП.СвРегФСС.СвОргФСС?.НаимФСС, КодФСС = documentXML.СвИП.СвРегФСС.СвОргФСС?.КодФСС, ДатаЗаписи = documentXML.СвИП.СвРегФСС.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвРегФСС.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвРегФСС.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvRegFSS).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.ЕГРИП_СвРегФСС.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRIPSvRegFSS";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(IPDB.ЕГРИП_СвРегФСС.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвИП.ИННФЛ;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.ИсторияИзменений.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.ЕГРИП_СвРегФСС = new List<EGRIPSvRegFSS> { new EGRIPSvRegFSS { Id = Guid.NewGuid().ToString(), ДатаРег = documentXML.СвИП.СвРегФСС.ДатаРег, РегНомФСС = documentXML.СвИП.СвРегФСС.РегНомФСС, НаимФСС = documentXML.СвИП.СвРегФСС.СвОргФСС?.НаимФСС, КодФСС = documentXML.СвИП.СвРегФСС.СвОргФСС?.КодФСС, ДатаЗаписи = documentXML.СвИП.СвРегФСС.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвРегФСС.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id } };
                            //}

                            break;

                        case "EGRIPSvAccountingNO":
                            if (documentXML.СвИП.СвУчетНО == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvAccountingNO previousEntry;
                                var newEntry = new EGRIPSvAccountingNO { Id = Guid.NewGuid().ToString(), ИННФЛ = documentXML.СвИП.СвУчетНО.ИННФЛ, ДатаПостУч = documentXML.СвИП.СвУчетНО.ДатаПостУч, НаимНО = documentXML.СвИП.СвУчетНО.СвНО.НаимНО, КодНО = documentXML.СвИП.СвУчетНО.СвНО.НаимНО, ДатаЗаписи = documentXML.СвИП.СвУчетНО.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвУчетНО.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id };

                                lock (listLockers[11])
                                {
                                    previousEntry = IPSubTables.ЕГРИП_СвУчетНО_Insert.Where(e => e.idЛицо == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[11])
                                    {
                                        if (previousEntry.ДатаЗаписи <= documentXML.СвИП.СвУчетНО.ГРНИПДата.ДатаЗаписи)
                                        {
                                            IPSubTables.ЕГРИП_СвУчетНО_Insert.RemoveAll(e => e.idЛицо == previousEntry.idЛицо);
                                            IPSubTables.ЕГРИП_СвУчетНО_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвУчетНО).Load();

                                        if (IPDB.ЕГРИП_СвУчетНО?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.ЕГРИП_СвУчетНО?.FirstOrDefault();
                                            IPSubTables.ЕГРИП_СвУчетНО_Delete.Add(IPDB.ЕГРИП_СвУчетНО?.FirstOrDefault().idЛицо.ToString());
                                        }
                                    }

                                    lock (listLockers[11])
                                    {
                                        IPSubTables.ЕГРИП_СвУчетНО_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.ДатаЗаписи <= documentXML.СвИП.СвУчетНО.ГРНИПДата.ДатаЗаписи)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvAccountingNO).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                Таблица = "EGRIPSvAccountingNO",
                                                Столбец = property.Name,
                                                ЗначениеДо = property.GetValue(previousEntry)?.ToString(),
                                                ЗначениеПосле = property.GetValue(newEntry)?.ToString(),
                                                ИНН = documentXML.СвИП.ИННФЛ,
                                                ДатаИзменения = DateTime.Now
                                            };

                                            // //_dbcontext.ИсторияИзменений.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.ИсторияИзменений_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.ЕГРИП_СвУчетНО).Load();

                            //if (IPDB.ЕГРИП_СвУчетНО.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.ЕГРИП_СвУчетНО.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвУчетНО.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.ЕГРИП_СвУчетНО = new List<EGRIPSvAccountingNO> { new EGRIPSvAccountingNO { Id = IPDB.ЕГРИП_СвУчетНО.FirstOrDefault()?.Id != null ? IPDB.ЕГРИП_СвУчетНО.FirstOrDefault().Id : Guid.NewGuid().ToString(), ИННФЛ = documentXML.СвИП.СвУчетНО.ИННФЛ, ДатаПостУч = documentXML.СвИП.СвУчетНО.ДатаПостУч, НаимНО = documentXML.СвИП.СвУчетНО.СвНО.НаимНО, КодНО = documentXML.СвИП.СвУчетНО.СвНО.НаимНО, ДатаЗаписи = documentXML.СвИП.СвУчетНО.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвУчетНО.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.ЕГРИП_СвУчетНО.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvAccountingNO).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.ЕГРИП_СвУчетНО.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.Таблица = "EGRIPSvAccountingNO";
                            //            changes.Столбец = property.Name;
                            //            changes.ЗначениеДо = property.GetValue(previousEntry)?.ToString();
                            //            changes.ЗначениеПосле = property.GetValue(IPDB.ЕГРИП_СвУчетНО.FirstOrDefault())?.ToString();
                            //            changes.ИНН = documentXML.СвИП.ИННФЛ;
                            //            changes.ДатаИзменения = DateTime.Now;

                            //            //_dbcontext.ИсторияИзменений.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.ИсторияИзменений.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.ЕГРИП_СвУчетНО = new List<EGRIPSvAccountingNO> { new EGRIPSvAccountingNO { Id = Guid.NewGuid().ToString(), ИННФЛ = documentXML.СвИП.СвУчетНО.ИННФЛ, ДатаПостУч = documentXML.СвИП.СвУчетНО.ДатаПостУч, НаимНО = documentXML.СвИП.СвУчетНО.СвНО.НаимНО, КодНО = documentXML.СвИП.СвУчетНО.СвНО.НаимНО, ДатаЗаписи = documentXML.СвИП.СвУчетНО.ГРНИПДата.ДатаЗаписи, ГРНИП = documentXML.СвИП.СвУчетНО.ГРНИПДата.ГРНИП, idЛицо = IPDB.Id } };
                            //}

                            break;
                    }
                }



                //try
                //{
                //    _dbcontext.SaveChanges();
                //}
                //catch (DbUpdateConcurrencyException ex)
                //{
                //    if (documentXML.retry == false)
                //    {
                //        documentXML.retry = true;
                //        Console.WriteLine($"Возникла ошибка параллельной обработки лица {documentXML.СвИП.ИННФЛ}, совершается вторая попытка парсинга");
                //        lock (locker)
                //        {
                //            if (entitiesInWork.Contains(documentXML.СвИП.ИННФЛ))
                //            {
                //                entitiesInWork.Remove(documentXML.СвИП.ИННФЛ);
                //            }
                //        }
                //        ParseIPDataDB((object)documentXML);
                //    }
                //}

                //try
                //{
                //    lock (locker)
                //    {
                //        if (entitiesInWork.Contains(documentXML.СвИП.ИННФЛ))
                //        {
                //            entitiesInWork.Remove(documentXML.СвИП.ИННФЛ);
                //        }
                //    }                  
                //}
                //catch (Exception e)
                //{

                //}               

                if (docCount - 1 == finishedWorkersCount)
                {
                    _dbcontext.ЕГРИП_ОКВЭД.Where(e => IPSubTables.ЕГРИП_ОКВЭД_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.ЕГРИП_ОКВЭД_Insert);

                    _dbcontext.ЕГРИП_СвУчетНО.Where(e => IPSubTables.ЕГРИП_СвУчетНО_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.ЕГРИП_СвУчетНО_Insert);

                    _dbcontext.ЕГРИП_СвАдрМЖ.Where(e => IPSubTables.ЕГРИП_СвАдрМЖ_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.ЕГРИП_СвАдрМЖ_Insert);

                    _dbcontext.ЕГРИП_СвФЛ.Where(e => IPSubTables.ЕГРИП_СвФЛ_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.ЕГРИП_СвФЛ_Insert);

                    _dbcontext.ЕГРИП_СвРегОрг.Where(e => IPSubTables.ЕГРИП_СвРегОрг_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.ЕГРИП_СвРегОрг_Insert);

                    _dbcontext.ЕГРИП_СвГражд.Where(e => IPSubTables.ЕГРИП_СвГражд_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.ЕГРИП_СвГражд_Insert);

                    _dbcontext.ЕГРИП_СвИП.Where(e => IPSubTables.ЕГРИП_СвИП_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.ЕГРИП_СвИП_Insert);

                    _dbcontext.ЕГРИП_СвЛицензия.Where(e => IPSubTables.ЕГРИП_СвЛицензия_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();

                    List<EGRIPSvLicense> svLicenseList = new List<EGRIPSvLicense>();
                    foreach (var entries in IPSubTables.ЕГРИП_СвЛицензия_Insert)
                    {
                        svLicenseList.Add(entries.Entry);
                    }

                    _dbcontext.BulkCopy(svLicenseList);

                    _dbcontext.ЕГРИП_СвПрекращ.Where(e => IPSubTables.ЕГРИП_СвПрекращ_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.ЕГРИП_СвПрекращ_Insert);

                    _dbcontext.ЕГРИП_СвРегФСС.Where(e => IPSubTables.ЕГРИП_СвРегФСС_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.ЕГРИП_СвРегФСС_Insert);

                    _dbcontext.ЕГРИП_СвРегИП.Where(e => IPSubTables.ЕГРИП_СвРегИП_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.ЕГРИП_СвРегИП_Insert);

                    _dbcontext.ЕГРИП_СвРегПФ.Where(e => IPSubTables.ЕГРИП_СвРегПФ_Delete.Contains(e.idЛицо.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.ЕГРИП_СвРегПФ_Insert);

                    _dbcontext.BulkCopy(IPSubTables.ИсторияИзменений_Insert);

                    IPSubTables.ClearIPSubTables();
                }

                lock (locker)
                {
                    finishedWorkersCount++;
                }
            }
        }

        public static class IPSubTables
        {
            public static List<IP> IPInProcessing = new List<IP>();

            public static List<EGRIPOKVED>? ЕГРИП_ОКВЭД_Insert = new List<EGRIPOKVED>();
            public static List<EGRIPSvAccountingNO>? ЕГРИП_СвУчетНО_Insert = new List<EGRIPSvAccountingNO>();
            public static List<EGRIPSvAdrMJ>? ЕГРИП_СвАдрМЖ_Insert = new List<EGRIPSvAdrMJ>();
            public static List<EGRIPSVFL>? ЕГРИП_СвФЛ_Insert = new List<EGRIPSVFL>();
            public static List<EGRIPSvGegOrg>? ЕГРИП_СвРегОрг_Insert = new List<EGRIPSvGegOrg>();
            public static List<EGRIPSvGrajd> ЕГРИП_СвГражд_Insert = new List<EGRIPSvGrajd>();
            public static List<EGRIPSvIP>? ЕГРИП_СвИП_Insert = new List<EGRIPSvIP>();
            public static List<(EGRIPSvLicense Entry, DateTime Date)> ЕГРИП_СвЛицензия_Insert = new List<(EGRIPSvLicense Entry, DateTime Date)>();
            public static List<EGRIPSvPrekras_> ЕГРИП_СвПрекращ_Insert = new List<EGRIPSvPrekras_>();
            public static List<EGRIPSvRegFSS> ЕГРИП_СвРегФСС_Insert = new List<EGRIPSvRegFSS>();
            public static List<EGRIPSvRegIP> ЕГРИП_СвРегИП_Insert = new List<EGRIPSvRegIP>();
            public static List<EGRIPSvRegPF> ЕГРИП_СвРегПФ_Insert = new List<EGRIPSvRegPF>();

            public static List<string?> ЕГРИП_ОКВЭД_Delete = new List<string?>();
            public static List<string?> ЕГРИП_СвУчетНО_Delete = new List<string?>();
            public static List<string?> ЕГРИП_СвАдрМЖ_Delete = new List<string?>();
            public static List<string?> ЕГРИП_СвФЛ_Delete = new List<string?>();
            public static List<string?> ЕГРИП_СвРегОрг_Delete = new List<string?>();
            public static List<string?> ЕГРИП_СвГражд_Delete = new List<string?>();
            public static List<string?> ЕГРИП_СвИП_Delete = new List<string?>();
            public static List<string?> ЕГРИП_СвЛицензия_Delete = new List<string?>();
            public static List<string?> ЕГРИП_СвПрекращ_Delete = new List<string?>();
            public static List<string?> ЕГРИП_СвРегФСС_Delete = new List<string?>();
            public static List<string?> ЕГРИП_СвРегИП_Delete = new List<string?>();
            public static List<string?> ЕГРИП_СвРегПФ_Delete = new List<string?>();

            public static List<ChangeLog> ИсторияИзменений_Insert = new List<ChangeLog>();

            public static void ClearIPSubTables()
            {
                IPSubTables.IPInProcessing.Clear();

                IPSubTables.ЕГРИП_ОКВЭД_Insert.Clear();
                IPSubTables.ЕГРИП_ОКВЭД_Delete.Clear();

                IPSubTables.ЕГРИП_СвУчетНО_Insert.Clear();
                IPSubTables.ЕГРИП_СвУчетНО_Delete.Clear();

                IPSubTables.ЕГРИП_СвАдрМЖ_Insert.Clear();
                IPSubTables.ЕГРИП_СвАдрМЖ_Delete.Clear();

                IPSubTables.ЕГРИП_СвФЛ_Insert.Clear();
                IPSubTables.ЕГРИП_СвФЛ_Delete.Clear();

                IPSubTables.ЕГРИП_СвРегОрг_Insert.Clear();
                IPSubTables.ЕГРИП_СвРегОрг_Delete.Clear();

                IPSubTables.ЕГРИП_СвГражд_Insert.Clear();
                IPSubTables.ЕГРИП_СвГражд_Delete.Clear();

                IPSubTables.ЕГРИП_СвИП_Insert.Clear();
                IPSubTables.ЕГРИП_СвИП_Delete.Clear();

                IPSubTables.ЕГРИП_СвЛицензия_Insert.Clear();
                IPSubTables.ЕГРИП_СвЛицензия_Delete.Clear();

                IPSubTables.ЕГРИП_СвПрекращ_Insert.Clear();
                IPSubTables.ЕГРИП_СвПрекращ_Delete.Clear();

                IPSubTables.ЕГРИП_СвРегФСС_Insert.Clear();
                IPSubTables.ЕГРИП_СвРегФСС_Delete.Clear();

                IPSubTables.ЕГРИП_СвРегИП_Insert.Clear();
                IPSubTables.ЕГРИП_СвРегИП_Delete.Clear();

                IPSubTables.ЕГРИП_СвРегПФ_Insert.Clear();
                IPSubTables.ЕГРИП_СвРегПФ_Delete.Clear();

                IPSubTables.ИсторияИзменений_Insert.Clear();
            }
        }

        public static class ULSubTables
        {
            public static List<UL> ULInProcessing = new List<UL>();

            public static List<EGRULOKVED> ЕГРЮЛ_ОКВЭД_Insert = new List<EGRULOKVED>();
            public static List<EGRULSvAddressUL> ЕГРЮЛ_СвАдресЮЛ_Insert = new List<EGRULSvAddressUL>();
            public static List<EGRULSvDerjRegistryAO> ЕГРЮЛ_СвДержРеестрАО_Insert = new List<EGRULSvDerjRegistryAO>();
            public static List<(EGRULSvShareOOO Entry, DateTime? Date)> ЕГРЮЛ_СвДоляООО_Insert = new List<(EGRULSvShareOOO Entry, DateTime? Date)>();
            public static List<EGRULSvZapEGRUL> ЕГРЮЛ_СвЗапЕГРЮЛ_Insert = new List<EGRULSvZapEGRUL>();
            public static List<EGRULSvLicense> ЕГРЮЛ_СвЛицензия_Insert = new List<EGRULSvLicense>();
            public static List<EGRULSvNaimUL> ЕГРЮЛ_СвНаимЮЛ_Insert = new List<EGRULSvNaimUL>();
            public static List<EGRULSvObrUL> ЕГРЮЛ_СвОбрЮЛ_Insert = new List<EGRULSvObrUL>();
            public static List<EGRULSvPodrazd> ЕГРЮЛ_СвПодразд_Insert = new List<EGRULSvPodrazd>();
            public static List<EGRULSvPredsh> ЕГРЮЛ_СвПредш_Insert = new List<EGRULSvPredsh>();
            public static List<EGRULSvPreem> ЕГРЮЛ_СвПреем_Insert = new List<EGRULSvPreem>();
            public static List<EGRULSvPrekrUL> ЕГРЮЛ_СвПрекрЮЛ_Insert = new List<EGRULSvPrekrUL>();
            public static List<EGRULSvRegOrg> ЕГРЮЛ_СвРегОрг_Insert = new List<EGRULSvRegOrg>();
            public static List<EGRULSvRegPF> ЕГРЮЛ_СвРегПФ_Insert = new List<EGRULSvRegPF>();
            public static List<EGRULSvRegFSS> ЕГРЮЛ_СвРегФСС_Insert = new List<EGRULSvRegFSS>();
            public static List<EGRULSvReorg> ЕГРЮЛ_СвРеорг_Insert = new List<EGRULSvReorg>();
            public static List<EGRULSvStatus> ЕГРЮЛ_СвСтатус_Insert = new List<EGRULSvStatus>();
            public static List<EGRULSvUstKap> ЕГРЮЛ_СвУстКап_Insert = new List<EGRULSvUstKap>();
            public static List<EGRULSvAccountingNO> ЕГРЮЛ_СвУчетНО_Insert = new List<EGRULSvAccountingNO>();
            public static List<EGRULSvFounder> ЕГРЮЛ_СвУчредит_Insert = new List<EGRULSvFounder>();
            public static List<EGRULSvUL> ЕГРЮЛ_СвЮЛ_Insert = new List<EGRULSvUL>();
            public static List<EGRULSvedDoljnFL> ЕГРЮЛ_СведДолжнФЛ_Insert = new List<EGRULSvedDoljnFL>();

            public static List<string?> ЕГРЮЛ_ОКВЭД_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвАдресЮЛ_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвДержРеестрАО_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвДоляООО_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвЗапЕГРЮЛ_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвЛицензия_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвНаимЮЛ_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвОбрЮЛ_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвПодразд_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвПредш_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвПреем_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвПрекрЮЛ_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвРегОрг_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвРегПФ_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвРегФСС_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвРеорг_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвСтатус_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвУстКап_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвУчетНО_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвУчредит_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СвЮЛ_Delete = new List<string?>();
            public static List<string?> ЕГРЮЛ_СведДолжнФЛ_Delete = new List<string?>();

            public static void ClearULSubTables()
            {
                ULSubTables.ULInProcessing.Clear();

                ULSubTables.ЕГРЮЛ_ОКВЭД_Insert.Clear();
                ULSubTables.ЕГРЮЛ_ОКВЭД_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвАдресЮЛ_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвАдресЮЛ_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвДержРеестрАО_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвДержРеестрАО_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвДоляООО_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвДоляООО_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СведДолжнФЛ_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СведДолжнФЛ_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвЗапЕГРЮЛ_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвЗапЕГРЮЛ_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвЛицензия_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвЛицензия_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвНаимЮЛ_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвНаимЮЛ_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвОбрЮЛ_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвОбрЮЛ_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвПодразд_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвПодразд_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвПредш_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвПредш_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвПреем_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвПреем_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвПрекрЮЛ_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвПрекрЮЛ_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвРегОрг_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвРегОрг_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвРегПФ_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвРегПФ_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвРегФСС_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвРегФСС_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвРеорг_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвРеорг_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвСтатус_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвСтатус_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвУстКап_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвУстКап_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвУчетНО_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвУчетНО_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвУчредит_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвУчредит_Delete.Clear();

                ULSubTables.ЕГРЮЛ_СвЮЛ_Insert.Clear();
                ULSubTables.ЕГРЮЛ_СвЮЛ_Delete.Clear();

                IPSubTables.ИсторияИзменений_Insert.Clear();
            }
        }
    }

}
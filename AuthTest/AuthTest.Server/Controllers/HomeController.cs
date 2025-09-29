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
                tableViewModel.�����_���� = _dbcontext.�����_����?.ToList();
                tableViewModel.�����_���� = _dbcontext.�����_����.ToList();
                tableViewModel.�����_����� = _dbcontext.�����_�����.ToList();
                tableViewModel.�����_������� = _dbcontext.�����_�������.ToList();
                tableViewModel.�����_������� = _dbcontext.�����_�������.ToList();
                tableViewModel.�����_���� = _dbcontext.�����_����.ToList();
                //tableViewModel.�����_���������� = _dbcontext.�����_����������.ToList();
                tableViewModel.�����_��������� = _dbcontext.�����_���������.ToList();
                tableViewModel.�����_������� = _dbcontext.�����_�������.ToList();
                tableViewModel.�����_�������� = _dbcontext.�����_��������?.ToList();
                tableViewModel.�����_������� = _dbcontext.�����_�������.ToList();
                tableViewModel.�����_�������� = _dbcontext.�����_��������.ToList();
                tableViewModel.�����_�������� = _dbcontext.�����_��������.ToList();
                tableViewModel.�����_����� = _dbcontext.�����_�����.ToList();
                //tableViewModel.�����_��������� = _dbcontext.�����_���������.ToList();
                tableViewModel.�����_�������������� = _dbcontext.�����_��������������?.ToList();
                tableViewModel.�����_��������� = _dbcontext.�����_���������?.ToList();
                tableViewModel.�����_���������� = _dbcontext.�����_����������.ToList();
                tableViewModel.�����_���������� = _dbcontext.�����_����������.ToList();
                tableViewModel.�����_�������� = _dbcontext.�����_��������.ToList();
                //tableViewModel.�����_������� = _dbcontext.�����_�������.ToList();
                tableViewModel.�����_��������� = _dbcontext.�����_���������.ToList();
                tableViewModel.�����_������� = _dbcontext.�����_�������.ToList();
                tableViewModel.�����_������� = _dbcontext.�����_�������.ToList();
                tableViewModel.�����_��������� = _dbcontext.�����_���������.ToList();
                tableViewModel.�����_�������� = _dbcontext.�����_��������?.ToList();
                tableViewModel.�����_������� = _dbcontext.�����_�������.ToList();
                tableViewModel.�����_�������� = _dbcontext.�����_��������.ToList();
                tableViewModel.�����_������� = _dbcontext.�����_�������.ToList();
                tableViewModel.�����_�������� = _dbcontext.�����_��������?.ToList();
                tableViewModel.�����_�������� = _dbcontext.�����_��������.ToList();
                tableViewModel.�����_��������� = _dbcontext.�����_���������.ToList();
                tableViewModel.�����_����������� = _dbcontext.�����_�����������.ToList();
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
        public async Task<IActionResult> ���������(IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (file == null || file.Length == 0)
            {
                return BadRequest("���� ������");
            }

            entitiesInWork.Clear();
            IPSubTables.ClearIPSubTables();
            ULSubTables.ClearULSubTables();
            finishedWorkersCount = 0;

            if (Path.GetExtension(file.FileName) == ".xml")
            {
                using var stream = new StreamReader(file.OpenReadStream(), Encoding.GetEncoding(1251));
                var serializer = new XmlSerializer(typeof(����));
                var model = serializer.Deserialize(stream) as ����;

                docCount += Convert.ToInt32(model.������);

                if (model.������.Equals("�����_����_����"))
                {
                    foreach (�������� document in model.��������)
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
                    foreach (�������� document in model.��������)
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
                // ��������� ���� �� �������
                var path = Path.Combine(Path.GetTempPath(), fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                using (var uploadedZip = ZipFile.Open(path, ZipArchiveMode.Read))
                {
                    // ���� �� ���� ������ � ������
                    foreach (var entry in uploadedZip.Entries)
                    {
                        using (var entryStream = entry.Open())
                        {
                            var serializer = new XmlSerializer(typeof(����));
                            var model = serializer.Deserialize(entryStream) as ����;

                            docCount += Convert.ToInt32(model.������);

                            if (model.������.Equals("�����_����_����"))
                            {
                                foreach (�������� document in model.��������)
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
                                foreach (�������� document in model.��������)
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
                return BadRequest("���������������� ������ �����");
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
                        tableViewModel.�����_���� = _dbcontext.�����_����.ToList();
                        tableViewModel.�����_���� = _dbcontext.�����_����.ToList();
                        return Json(tableViewModel);
                    case "IPtable":
                        tableViewModel.�����_���� = _dbcontext.�����_����.ToList();
                        return Json(tableViewModel);
                    case "ULtable":
                        tableViewModel.�����_���� = _dbcontext.�����_����.ToList();
                        return Json(tableViewModel);
                    default:
                        return BadRequest("������������ buttonId");
                }
            }
        }


        [HttpGet]
        public IActionResult Details(string table, int id)
        {
            List<dynamic> details = new List<dynamic>();

            using (DbContextTable _dbcontext = _contextFactory.CreateDbContext())
            {
                //�������� ������ �� ������� table
                var dbSet = _dbcontext.Set(Type.GetType("EgrWebEntity.ModelTable." + table + ", EgrWebEntity, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null"));
                foreach (dynamic entity in dbSet)
                {
                    //�������� � ���������� ����� ����� ���� ���������� � ���� id����
                    var entry = (IGenericTable)entity;

                    if (entry.id���� == id)
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
                logs.AddRange(from l in _dbcontext.���������������� where l.������� == table && l.��� == INN select l);
            }

            return Json(logs);
        }

        [HttpGet]
        public IActionResult GetINNByEntityID(int id)
        {
            string inn;

            using (DbContextTable _dbcontext = _contextFactory.CreateDbContext())
            {
                inn = (from u in _dbcontext.������ where u.Id == id select u).FirstOrDefault().���;
            }

            return Json(inn);
        }

        public async void ParseULDataDB(object documentObject)
        {
            �������� documentXML = (��������)documentObject;
            bool firstTime = false;

            if (documentXML.���� == null)
            {
                return;
            }

            // ���� � ������� �� �������� ������ ������� ���� ���� ���������� ���������
            //while (entitiesInWork.Contains(documentXML.����.���))
            //{
            //    Console.WriteLine($"���� {documentXML.����.���} ��� � ���������, �������� ����������.");
            //    Thread.Sleep(5000);
            //}
            // ��������� �� � ������ ��������������
            // entitiesInWork.Add(documentXML.����.���);

            using (DbContextTable _dbcontext = _contextFactory.CreateDbContext())
            {
                //������� ������ ��� ������ � ���������� �� � ��
                UL ULDB = new UL();

                //���� ������ � ��
                UL data = (from u in _dbcontext.������ where u.��� == documentXML.����.��� select u).FirstOrDefault();
                if (data != null)
                {
                    ULDB = data;
                }
                else
                {
                    lock (locker)
                    {
                        data = ULSubTables.ULInProcessing.Where(e => e.��� == documentXML.����.���).FirstOrDefault();
                    }

                    if (data != null)
                    {
                        ULDB = data;
                    }
                    else
                    {
                        //���� ������ ��� ��������� ���� �������� ������ �� xml
                        ULDB = new UL { �������� = documentXML.����.��������, ���� = documentXML.����.����, ��� = documentXML.����.���, ��� = documentXML.����.���, ������ = documentXML.����.������, ������ = documentXML.����.������, ����������� = documentXML.����.�����������, ����� = documentXML.����� };
                        firstTime = true;
                        _dbcontext.������.Add(ULDB);
                        //��������� ������ � �� (����� ������� ������� ���� � ����������� �������)
                        _dbcontext.SaveChanges();

                        lock (locker)
                        {
                            ULSubTables.ULInProcessing.Add(ULDB);
                        }
                    }

                }

                //��������� ������ � xml ���������
                Document documentDB = new Document();
                documentDB.������������ = DateTime.Now;
                documentDB.����� = documentXML.�����;
                documentDB.id�� = ULDB.Id;

                ULDB.document = new List<Document> { documentDB };
                _dbcontext.������.Update(ULDB);
                _dbcontext.SaveChanges();

                //�������� �� ���� �������� �����������
                foreach (var entity in _dbcontext.Model.GetEntityTypes())
                {
                    switch (entity.ShortName())
                    {
                        case "EGRULOKVED":
                            if (documentXML.����.������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                List<EGRULOKVED> previousEntries;
                                var currentEntries = new List<EGRULOKVED>();
                                var currentEntry = new EGRULOKVED { Id = Guid.NewGuid().ToString(), ������ = documentXML.����.�������?.����������?.�����������, ����� = documentXML.����.�������?.����������?.�������.���, ��������� = documentXML.����.�������?.����������?.�������.����������, �������� = documentXML.����.�������?.����������?.��������, ������������ = documentXML.����.�������?.����������?.���������, ������ = true, id���� = ULDB.Id };
                                DateTime? currentEntriesDate = documentXML.����.�������.���������� != null ? documentXML.����.�������.����������?.�������.���������� : documentXML.����.�������.����������?.FirstOrDefault().�������.����������;

                                lock (listLockers[0])
                                {
                                    previousEntries = ULSubTables.�����_�����_Insert.Where(e => e.id���� == ULDB.Id).ToList();

                                    if (previousEntries.Count != 0)
                                    {
                                        if (previousEntries.FirstOrDefault()?.��������� <= currentEntriesDate)
                                        {
                                            ULSubTables.�����_�����_Insert.RemoveAll(e => e.id���� == ULDB.Id);
                                            ULSubTables.�����_�����_Insert.Add(currentEntry);

                                            foreach (���������� okvedDop in documentXML.����.�������?.����������)
                                            {
                                                currentEntry = new EGRULOKVED { Id = Guid.NewGuid().ToString(), �������� = okvedDop.��������, ������ = false, ������������ = okvedDop.���������, ������ = okvedDop.�����������, ����� = okvedDop.�������.���, ��������� = okvedDop.�������.����������, id���� = ULDB.Id };
                                                currentEntries.Add(currentEntry);
                                                ULSubTables.�����_�����_Insert.Add(currentEntry);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (firstTime == false)
                                        {
                                            _dbcontext.Entry(ULDB).Collection(u => u.�����_�����).Load();

                                            if (ULDB.�����_�����?.FirstOrDefault() != null)
                                            {
                                                previousEntries = ULDB.�����_�����.ToList();
                                                ULSubTables.�����_�����_Delete.Add(ULDB.�����_�����?.FirstOrDefault().id����.ToString());
                                            }
                                        }

                                        ULSubTables.�����_�����_Insert.Add(currentEntry);

                                        foreach (���������� okvedDop in documentXML.����.�������?.����������)
                                        {
                                            currentEntry = new EGRULOKVED { Id = Guid.NewGuid().ToString(), �������� = okvedDop.��������, ������ = false, ������������ = okvedDop.���������, ������ = okvedDop.�����������, ����� = okvedDop.�������.���, ��������� = okvedDop.�������.����������, id���� = ULDB.Id };
                                            currentEntries.Add(currentEntry);
                                            ULSubTables.�����_�����_Insert.Add(currentEntry);
                                        }
                                    }
                                }


                                if (previousEntries?.Count > 0 && previousEntries.FirstOrDefault()?.��������� <= currentEntriesDate)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var oldEntry in previousEntries)
                                    {
                                        var newEntry = currentEntries.Where(e => e.�������� == oldEntry.�������� || e.������������ == oldEntry.������������).FirstOrDefault();

                                        if (newEntry == null)
                                        {
                                            continue;
                                        }

                                        foreach (var property in typeof(EGRULOKVED).GetProperties())
                                        {
                                            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                            {
                                                if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                    continue;

                                                ChangeLog changes = new ChangeLog();

                                                changes.������� = "EGRULOKVED";
                                                changes.������� = property.Name;
                                                changes.���������� = property.GetValue(oldEntry)?.ToString();
                                                changes.������������� = property.GetValue(newEntry)?.ToString();
                                                changes.��� = documentXML.����.���;
                                                changes.������������� = DateTime.Now;

                                                //_dbcontext.����������������.Add(changes);
                                                changeList.Add(changes);
                                            }
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            ////���������� ������ ������� ������� ��� ����, ������� ������ ������������
                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_�����).Load();

                            //if (ULDB.�����_�����?.Count > 0)
                            //{
                            //    foreach (EGRULOKVED entry in ULDB.�����_�����)
                            //    {
                            //        //�������� ������ ������ � �� �� ��������
                            //        _dbcontext.Entry(entry).State = EntityState.Deleted;
                            //    }

                            //    //��������� ������ ������ ��� ������� ���������
                            //    var previousEntries = ULDB.�����_�����;
                            //    //���������� �������� �����
                            //    ULDB.�����_����� = new List<EGRULOKVED> { new EGRULOKVED { Id = Guid.NewGuid().ToString(), ������ = documentXML.����.�������?.����������?.�����������, ����� = documentXML.����.�������?.����������?.�������.���, ��������� = documentXML.����.�������?.����������?.�������.����������, �������� = documentXML.����.�������?.����������?.��������, ������������ = documentXML.����.�������?.����������?.���������, ������ = true, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_�����.FirstOrDefault()).State = EntityState.Added;

                            //    //���������� ��� ������
                            //    foreach (���������� okvedDop in documentXML.����.�������?.����������)
                            //    {
                            //        ULDB.�����_�����.Add(new EGRULOKVED { Id = Guid.NewGuid().ToString(), �������� = okvedDop.��������, ������ = false, ������������ = okvedDop.���������, ������ = okvedDop.�����������, ����� = okvedDop.�������.���, ��������� = okvedDop.�������.����������, id���� = ULDB.Id });
                            //        _dbcontext.Entry(ULDB.�����_�����.Last()).State = EntityState.Added;
                            //    }

                            //    var currentEntries = ULDB.�����_�����;

                            //    foreach (var oldEntry in previousEntries)
                            //    {
                            //        //������������ ������ ������ � ����� (���� ������� �������)
                            //        var newEntry = currentEntries.Where(e => e.�������� == oldEntry.�������� || e.������������ == oldEntry.������������).FirstOrDefault();

                            //        if (newEntry == null)
                            //        {
                            //            continue;
                            //        }

                            //        //���� �� ���� ����� ������ ������� �������
                            //        foreach (var property in typeof(EGRULOKVED).GetProperties())
                            //        {
                            //            //���������� ����������� ���� �������
                            //            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                            //            {
                            //                if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                            //                    continue;

                            //                //���������� ���������
                            //                ChangeLog changes = new ChangeLog();

                            //                changes.������� = "EGRULOKVED";
                            //                changes.������� = property.Name;
                            //                changes.���������� = property.GetValue(oldEntry)?.ToString();
                            //                changes.������������� = property.GetValue(newEntry)?.ToString();
                            //                changes.��� = documentXML.����.���;
                            //                changes.������������� = DateTime.Now;

                            //                //_dbcontext.����������������.Add(changes);
                            //            }
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    //���� �� ������� �������, � ���� ��� ������� ������ ���������� ��� ��� ����
                            //    ULDB.�����_����� = new List<EGRULOKVED> { new EGRULOKVED { Id = Guid.NewGuid().ToString(), ������ = documentXML.����.�������?.����������?.�����������, ����� = documentXML.����.�������?.����������?.�������.���, ��������� = documentXML.����.�������?.����������?.�������.����������, �������� = documentXML.����.�������?.����������?.��������, ������������ = documentXML.����.�������?.����������?.���������, ������ = true, id���� = ULDB.Id } };

                            //    foreach (���������� okvedDop in documentXML.����.�������?.����������)
                            //    {
                            //        ULDB.�����_�����.Add(new EGRULOKVED { Id = Guid.NewGuid().ToString(), �������� = okvedDop.��������, ������ = false, ������������ = okvedDop.���������, ������ = okvedDop.�����������, ����� = okvedDop.�������.���, ��������� = okvedDop.�������.����������, id���� = ULDB.Id });
                            //    }
                            //}

                            break;

                        case "EGRULSvAddressUL":
                            if (documentXML.����.��������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvAddressUL previousEntry;
                                var newEntry = new EGRULSvAddressUL { Id = Guid.NewGuid().ToString(), ������ = documentXML.����.���������.�������?.������, ����� = documentXML.����.���������.�������?.�����, ��� = documentXML.����.���������.�������?.���, ����������� = documentXML.����.���������.�������?.�����������, ��������� = documentXML.����.���������.�������?.���������, ���������� = documentXML.����.���������.�������?.������?.����������, ��������� = documentXML.����.���������.�������?.�����?.���������, �������� = documentXML.����.���������.�������?.�����?.��������, ��������� = documentXML.����.���������.�������?.�����?.���������, �������� = documentXML.����.���������.�������?.�����?.��������, ��� = documentXML.����.���������.�������?.�������.���, ���������� = documentXML.����.���������.�������?.�������.����������, �������������� = documentXML.����.���������.�������?.����������?.��������������, ������������� = documentXML.����.���������.�������?.����������?.�������������, ��������� = documentXML.����.���������.�������?.������?.���������, ��������� = documentXML.����.���������.�������?.�����?.���������, �������� = documentXML.����.���������.�������?.�����?.��������, ��������������� = documentXML.����.���������.������������?.���������������, ��������������� = documentXML.����.���������.������������?.���������������, id���� = ULDB.Id };

                                lock (listLockers[1])
                                {
                                    previousEntry = ULSubTables.�����_���������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[1])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.���������.�������?.�������.����������)
                                        {
                                            ULSubTables.�����_���������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_���������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_���������).Load();

                                        if (ULDB.�����_���������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_���������?.FirstOrDefault();
                                            ULSubTables.�����_���������_Delete.Add(ULDB.�����_���������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[1])
                                    {
                                        ULSubTables.�����_���������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.���������.�������?.�������.����������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRULSvAddressUL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new()
                                            {
                                                ������� = "EGRULSvAddressUL",
                                                ������� = property.Name,
                                                ���������� = property.GetValue(previousEntry)?.ToString(),
                                                ������������� = property.GetValue(newEntry)?.ToString(),
                                                ��� = documentXML.����.���,
                                                ������������� = DateTime.Now
                                            };

                                            // //_dbcontext.����������������.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_���������).Load();

                            //if (ULDB.�����_���������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_���������.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_���������.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_��������� = new List<EGRULSvAddressUL> { new EGRULSvAddressUL { Id = ULDB.�����_���������.FirstOrDefault()?.Id != null ? ULDB.�����_���������.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ������ = documentXML.����.���������.�������?.������, ����� = documentXML.����.���������.�������?.�����, ��� = documentXML.����.���������.�������?.���, ����������� = documentXML.����.���������.�������?.�����������, ��������� = documentXML.����.���������.�������?.���������, ���������� = documentXML.����.���������.�������?.������?.����������, ��������� = documentXML.����.���������.�������?.�����?.���������, �������� = documentXML.����.���������.�������?.�����?.��������, ��������� = documentXML.����.���������.�������?.�����?.���������, �������� = documentXML.����.���������.�������?.�����?.��������, ��� = documentXML.����.���������.�������?.�������.���, ���������� = documentXML.����.���������.�������?.�������.����������, �������������� = documentXML.����.���������.�������?.����������?.��������������, ������������� = documentXML.����.���������.�������?.����������?.�������������, ��������� = documentXML.����.���������.�������?.������?.���������, ��������� = documentXML.����.���������.�������?.�����?.���������, �������� = documentXML.����.���������.�������?.�����?.��������, ��������������� = documentXML.����.���������.������������?.���������������, ��������������� = documentXML.����.���������.������������?.���������������, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_���������.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvAddressUL).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_���������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvAddressUL";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_���������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_��������� = new List<EGRULSvAddressUL> { new EGRULSvAddressUL { Id = Guid.NewGuid().ToString(), ������ = documentXML.����.���������.�������?.������, ����� = documentXML.����.���������.�������?.�����, ��� = documentXML.����.���������.�������?.���, ����������� = documentXML.����.���������.�������?.�����������, ��������� = documentXML.����.���������.�������?.���������, ���������� = documentXML.����.���������.�������?.������?.����������, ��������� = documentXML.����.���������.�������?.�����?.���������, �������� = documentXML.����.���������.�������?.�����?.��������, ��������� = documentXML.����.���������.�������?.�����?.���������, �������� = documentXML.����.���������.�������?.�����?.��������, ��� = documentXML.����.���������.�������?.�������.���, ���������� = documentXML.����.���������.�������?.�������.����������, �������������� = documentXML.����.���������.�������?.����������?.��������������, ������������� = documentXML.����.���������.�������?.����������?.�������������, ��������� = documentXML.����.���������.�������?.������?.���������, ��������� = documentXML.����.���������.�������?.�����?.���������, �������� = documentXML.����.���������.�������?.�����?.��������, ��������������� = documentXML.����.���������.������������?.���������������, ��������������� = documentXML.����.���������.������������?.���������������, id���� = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvDerjRegistryAO":
                            if (documentXML.����.�������������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvDerjRegistryAO previousEntry;
                                var newEntry = new EGRULSvDerjRegistryAO { Id = Guid.NewGuid().ToString(), ��� = documentXML.����.��������������?.������������?.���, ���� = documentXML.����.��������������?.������������?.����, ���������� = documentXML.����.��������������?.������������?.����������, ���������� = documentXML.����.��������������?.������������?.�������.����������, ��� = documentXML.����.��������������?.������������?.�������.���, id���� = ULDB.Id };

                                lock (listLockers[2])
                                {
                                    previousEntry = ULSubTables.�����_��������������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[2])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.��������������.������������.�������.����������)
                                        {
                                            ULSubTables.�����_��������������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_��������������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_��������������).Load();

                                        if (ULDB.�����_��������������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_��������������?.FirstOrDefault();
                                            ULSubTables.�����_��������������_Delete.Add(ULDB.�����_��������������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[2])
                                    {
                                        ULSubTables.�����_��������������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.��������������.������������.�������.����������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRULSvDerjRegistryAO).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new()
                                            {
                                                ������� = "EGRULSvDerjRegistryAO",
                                                ������� = property.Name,
                                                ���������� = property.GetValue(previousEntry)?.ToString(),
                                                ������������� = property.GetValue(newEntry)?.ToString(),
                                                ��� = documentXML.����.���,
                                                ������������� = DateTime.Now
                                            };

                                            // //_dbcontext.����������������.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_��������������).Load();

                            //if (ULDB.�����_��������������?.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_��������������?.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_��������������?.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_�������������� = new List<EGRULSvDerjRegistryAO> { new EGRULSvDerjRegistryAO { Id = ULDB.�����_��������������?.FirstOrDefault()?.Id != null ? ULDB.�����_��������������?.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ��� = documentXML.����.��������������?.������������?.���, ���� = documentXML.����.��������������?.������������?.����, ���������� = documentXML.����.��������������?.������������?.����������, ���������� = documentXML.����.��������������?.������������?.�������.����������, ��� = documentXML.����.��������������?.������������?.�������.���, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_��������������?.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvDerjRegistryAO).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_��������������?.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvDerjRegistryAO";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_��������������?.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_�������������� = new List<EGRULSvDerjRegistryAO> { new EGRULSvDerjRegistryAO { Id = Guid.NewGuid().ToString(), ��� = documentXML.����.��������������?.������������?.���, ���� = documentXML.����.��������������?.������������?.����, ���������� = documentXML.����.��������������?.������������?.����������, ���������� = documentXML.����.��������������?.������������?.�������.����������, ��� = documentXML.����.��������������?.������������?.�������.���, id���� = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvShareOOO":
                            if (documentXML.����.��������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvShareOOO previousEntry;
                                (EGRULSvShareOOO Entry, DateTime? Date) EntryDatePair;
                                var newEntry = new EGRULSvShareOOO { Id = Guid.NewGuid().ToString(), ���������� = documentXML.����.���������?.����������, id���� = ULDB.Id };

                                lock (listLockers[3])
                                {
                                    EntryDatePair = ULSubTables.�����_���������_Insert.Where(e => e.Entry.id���� == ULDB.Id).FirstOrDefault();
                                    previousEntry = EntryDatePair.Entry;
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[3])
                                    {
                                        if (EntryDatePair.Date <= documentXML.����.���������.�������.����������)
                                        {
                                            ULSubTables.�����_���������_Insert.RemoveAll(e => e.Entry.id���� == previousEntry.id����);
                                            ULSubTables.�����_���������_Insert.Add((newEntry, documentXML.����.���������?.�������.����������));
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_���������).Load();

                                        if (ULDB.�����_���������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_���������?.FirstOrDefault();
                                            ULSubTables.�����_���������_Delete.Add(ULDB.�����_���������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[3])
                                    {
                                        ULSubTables.�����_���������_Insert.Add((newEntry, documentXML.����.���������?.�������.����������));
                                    }
                                }

                                if (previousEntry != null && EntryDatePair.Date <= documentXML.����.���������.�������.����������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRULSvShareOOO).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new()
                                            {
                                                ������� = "EGRULSvShareOOO",
                                                ������� = property.Name,
                                                ���������� = property.GetValue(previousEntry)?.ToString(),
                                                ������������� = property.GetValue(newEntry)?.ToString(),
                                                ��� = documentXML.����.���,
                                                ������������� = DateTime.Now
                                            };

                                            // //_dbcontext.����������������.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_���������).Load();

                            //if (ULDB.�����_���������?.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_���������?.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_���������?.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_��������� = new List<EGRULSvShareOOO> { new EGRULSvShareOOO { Id = ULDB.�����_���������?.FirstOrDefault()?.Id != null ? ULDB.�����_���������?.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ���������� = documentXML.����.���������?.����������, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_���������?.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvShareOOO).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_���������?.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvShareOOO";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_���������?.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_��������� = new List<EGRULSvShareOOO> { new EGRULSvShareOOO { Id = Guid.NewGuid().ToString(), ���������� = documentXML.����.���������?.����������, id���� = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvZapEGRUL":
                            if (documentXML.����.���������� == null || documentXML.����.����������.Count == 0)
                            {
                                continue;
                            }
                            else
                            {
                                List<EGRULSvZapEGRUL> previousEntries;
                                var currentEntries = new List<EGRULSvZapEGRUL>();
                                DateTime? currentEntriesDate = documentXML.����.����������.Select(d => d.�������).ToArray()?.Max();

                                lock (listLockers[4])
                                {
                                    previousEntries = ULSubTables.�����_����������_Insert.Where(e => e.id���� == ULDB.Id).ToList();

                                    if (previousEntries.Count != 0)
                                    {
                                        if (previousEntries.Select(d => d.�������).ToArray()?.Max() <= currentEntriesDate)
                                        {
                                            ULSubTables.�����_����������_Insert.RemoveAll(e => e.id���� == previousEntries.FirstOrDefault().id����);

                                            foreach (���������� zapEGRUL in documentXML.����.����������)
                                            {
                                                foreach (����������� svedPredDoc in zapEGRUL.�����������)
                                                {
                                                    EGRULSvZapEGRUL zapEGRULBD = new EGRULSvZapEGRUL { Id = Guid.NewGuid().ToString(), ��� = zapEGRUL.���, ������� = zapEGRUL.�������, ����� = zapEGRUL.�����, ���������� = zapEGRUL.������?.����������, ������� = zapEGRUL.������?.�������, ������ = zapEGRUL.��������?.������, ����� = zapEGRUL.��������?.�����, id���� = ULDB.Id };
                                                    zapEGRULBD.������� = svedPredDoc.�������;
                                                    zapEGRULBD.������ = svedPredDoc.������;
                                                    zapEGRULBD.������� = svedPredDoc.�������;

                                                    if (zapEGRUL.������ != null)
                                                    {
                                                        zapEGRULBD.����� = zapEGRUL.������.�����;
                                                        zapEGRULBD.����� = zapEGRUL.������.�����;
                                                        zapEGRULBD.����������� = zapEGRUL.������.�����������;
                                                    }

                                                    ULSubTables.�����_����������_Insert.Add(zapEGRULBD);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (firstTime == false)
                                        {
                                            _dbcontext.Entry(ULDB).Collection(u => u.�����_����������).Load();

                                            if (ULDB.�����_����������?.FirstOrDefault() != null)
                                            {
                                                //previousEntries = ULDB.�����_����������.ToList();
                                                ULSubTables.�����_����������_Delete.Add(ULDB.�����_����������?.FirstOrDefault().id����.ToString());
                                            }
                                        }

                                        foreach (���������� zapEGRUL in documentXML.����.����������)
                                        {
                                            foreach (����������� svedPredDoc in zapEGRUL.�����������)
                                            {
                                                EGRULSvZapEGRUL zapEGRULBD = new EGRULSvZapEGRUL { Id = Guid.NewGuid().ToString(), ��� = zapEGRUL.���, ������� = zapEGRUL.�������, ����� = zapEGRUL.�����, ���������� = zapEGRUL.������?.����������, ������� = zapEGRUL.������?.�������, ������ = zapEGRUL.��������?.������, ����� = zapEGRUL.��������?.�����, id���� = ULDB.Id };
                                                zapEGRULBD.������� = svedPredDoc.�������;
                                                zapEGRULBD.������ = svedPredDoc.������;
                                                zapEGRULBD.������� = svedPredDoc.�������;

                                                if (zapEGRUL.������ != null)
                                                {
                                                    zapEGRULBD.����� = zapEGRUL.������.�����;
                                                    zapEGRULBD.����� = zapEGRUL.������.�����;
                                                    zapEGRULBD.����������� = zapEGRUL.������.�����������;
                                                }

                                                ULSubTables.�����_����������_Insert.Add(zapEGRULBD);
                                            }
                                        }
                                    }
                                }

                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_����������).Load();

                            //if (ULDB.�����_����������?.Count > 0)
                            //{
                            //    foreach (var entry in ULDB.�����_����������)
                            //    {
                            //        _dbcontext.Entry(entry).State = EntityState.Deleted;
                            //    }

                            //    //var previousEntries = ULDB.�����_����������;

                            //    foreach (���������� zapEGRUL in documentXML.����.����������)
                            //    {
                            //        foreach (����������� svedPredDoc in zapEGRUL.�����������)
                            //        {
                            //            EGRULSvZapEGRUL zapEGRULBD = new EGRULSvZapEGRUL { Id = Guid.NewGuid().ToString(), ��� = zapEGRUL.���, ������� = zapEGRUL.�������, ����� = zapEGRUL.�����, ���������� = zapEGRUL.������?.����������, ������� = zapEGRUL.������?.�������, ������ = zapEGRUL.��������?.������, ����� = zapEGRUL.��������?.�����, id���� = ULDB.Id };
                            //            zapEGRULBD.������� = svedPredDoc.�������;
                            //            zapEGRULBD.������ = svedPredDoc.������;
                            //            zapEGRULBD.������� = svedPredDoc.�������;

                            //            if (zapEGRUL.������ != null)
                            //            {
                            //                zapEGRULBD.����� = zapEGRUL.������.�����;
                            //                zapEGRULBD.����� = zapEGRUL.������.�����;
                            //                zapEGRULBD.����������� = zapEGRUL.������.�����������;
                            //            }

                            //            ULDB.�����_����������.Add(zapEGRULBD);
                            //            _dbcontext.Entry(ULDB.�����_����������.Last()).State = EntityState.Added;
                            //        }
                            //    }

                            //    //var currentEntries = ULDB.�����_����������;

                            //    //foreach (var oldEntry in previousEntries)
                            //    //{
                            //    //    var newEntry = currentEntries.Where(e => e.�������� == oldEntry.�������� || e.������������ == oldEntry.������������).FirstOrDefault();

                            //    //    if (newEntry == null)
                            //    //    {
                            //    //        continue;
                            //    //    }

                            //    //    foreach (var property in typeof(EGRULOKVED).GetProperties())
                            //    //    {
                            //    //        if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                            //    //        {
                            //    //            if (property.Name == "Id" || property.Name == "id����" || property.Name == "��")
                            //    //                continue;

                            //    //            ChangeLog changes = new ChangeLog();

                            //    //            changes.������� = "EGRULOKVED";
                            //    //            changes.������� = property.Name;
                            //    //            changes.���������� = property.GetValue(oldEntry)?.ToString();
                            //    //            changes.������������� = property.GetValue(newEntry)?.ToString();
                            //    //            changes.��� = documentXML.����.���;
                            //    //            changes.������������� = DateTime.Now;

                            //    //            //_dbcontext.����������������.Add(changes);
                            //    //        }
                            //    //    }
                            //    //}
                            //}
                            //else
                            //{
                            //    ULDB.�����_���������� = new List<EGRULSvZapEGRUL>();

                            //    foreach (���������� zapEGRUL in documentXML.����.����������)
                            //    {
                            //        foreach (����������� svedPredDoc in zapEGRUL.�����������)
                            //        {
                            //            EGRULSvZapEGRUL zapEGRULBD = new EGRULSvZapEGRUL { Id = Guid.NewGuid().ToString(), ��� = zapEGRUL.���, ������� = zapEGRUL.�������, ����� = zapEGRUL.�����, ���������� = zapEGRUL.������?.����������, ������� = zapEGRUL.������?.�������, ������ = zapEGRUL.��������?.������, ����� = zapEGRUL.��������?.�����, id���� = ULDB.Id };
                            //            zapEGRULBD.������� = svedPredDoc.�������;
                            //            zapEGRULBD.������ = svedPredDoc.������;
                            //            zapEGRULBD.������� = svedPredDoc.�������;

                            //            if (zapEGRUL.������ != null)
                            //            {
                            //                zapEGRULBD.����� = zapEGRUL.������.�����;
                            //                zapEGRULBD.����� = zapEGRUL.������.�����;
                            //                zapEGRULBD.����������� = zapEGRUL.������.�����������;
                            //            }

                            //            ULDB.�����_����������.Add(zapEGRULBD);
                            //        }
                            //    }
                            //}

                            break;

                        case "EGRULSvLicense":
                            if (documentXML.����.���������� == null || documentXML.����.����������.Count == 0)
                            {
                                continue;
                            }
                            else
                            {
                                List<EGRULSvLicense> previousEntries;
                                var currentEntries = new List<EGRULSvLicense>();
                                var dateArray = documentXML.����.����������.Where(x => x.������� != null).Select(d => d.�������.����������).ToArray();
                                DateTime? currentEntriesDate = dateArray.Length != 0 ? dateArray.Max() : null;

                                lock (listLockers[5])
                                {
                                    previousEntries = ULSubTables.�����_����������_Insert.Where(e => e.id���� == ULDB.Id).ToList();
                                }
                                if (previousEntries.Count != 0)
                                {
                                    lock (listLockers[5])
                                    {
                                        if (previousEntries.Select(d => d.����������).ToArray()?.Max() <= currentEntriesDate)
                                        {
                                            ULSubTables.�����_����������_Insert.RemoveAll(e => e.id���� == ULDB.Id);

                                            foreach (var svLicense in documentXML.����.����������)
                                            {
                                                ULSubTables.�����_����������_Insert.Add(new EGRULSvLicense { Id = Guid.NewGuid().ToString(), ���������� = svLicense.����������, ������� = svLicense.�������, ������ = svLicense.������, �������������� = svLicense.��������������, ������������ = svLicense.������������, ������������ = svLicense.������������, ���������� = svLicense.�������?.����������, ��� = svLicense.�������?.���, id���� = ULDB.Id });
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_����������).Load();

                                        if (ULDB.�����_����������?.FirstOrDefault() != null)
                                        {
                                            previousEntries = ULDB.�����_����������.ToList();
                                            ULSubTables.�����_����������_Delete.Add(ULDB.�����_����������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[5])
                                    {
                                        foreach (var svLicense in documentXML.����.����������)
                                        {
                                            ULSubTables.�����_����������_Insert.Add(new EGRULSvLicense { Id = Guid.NewGuid().ToString(), ���������� = svLicense.����������, ������� = svLicense.�������, ������ = svLicense.������, �������������� = svLicense.��������������, ������������ = svLicense.������������, ������������ = svLicense.������������, ���������� = svLicense.�������?.����������, ��� = svLicense.�������?.���, id���� = ULDB.Id });
                                        }
                                    }
                                }

                                if (previousEntries?.Count > 0 && previousEntries.Select(d => d.����������).ToArray()?.Max() <= currentEntriesDate)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var oldEntry in previousEntries)
                                    {
                                        var newEntry = currentEntries.Where(e => e.������ == oldEntry.������).FirstOrDefault();

                                        if (newEntry == null)
                                        {
                                            continue;
                                        }

                                        foreach (var property in typeof(EGRULSvLicense).GetProperties())
                                        {
                                            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                            {
                                                if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                    continue;

                                                ChangeLog changes = new ChangeLog();

                                                changes.������� = "EGRULSvLicense";
                                                changes.������� = property.Name;
                                                changes.���������� = property.GetValue(oldEntry)?.ToString();
                                                changes.������������� = property.GetValue(newEntry)?.ToString();
                                                changes.��� = documentXML.����.���;
                                                changes.������������� = DateTime.Now;

                                                changeList.Add(changes);
                                            }
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_����������).Load();

                            //if (ULDB.�����_����������?.Count > 0)
                            //{
                            //    foreach (var entry in ULDB.�����_����������)
                            //    {
                            //        _dbcontext.Entry(entry).State = EntityState.Deleted;
                            //    }

                            //    var previousEntries = ULDB.�����_����������;

                            //    ULDB.�����_���������� = new List<EGRULSvLicense>();

                            //    foreach (var svLicense in documentXML.����.����������)
                            //    {
                            //        ULDB.�����_����������.Add(new EGRULSvLicense { Id = Guid.NewGuid().ToString(), ���������� = svLicense.����������, ������� = svLicense.�������, ������ = svLicense.������, �������������� = svLicense.��������������, ������������ = svLicense.������������, ������������ = svLicense.������������, ���������� = svLicense.�������.����������, ��� = svLicense.�������.���, id���� = ULDB.Id });
                            //        _dbcontext.Entry(ULDB.�����_����������.Last()).State = EntityState.Added;
                            //    }

                            //    var currentEntries = ULDB.�����_����������;

                            //    foreach (var oldEntry in previousEntries)
                            //    {
                            //        var newEntry = currentEntries.Where(e => e.������ == oldEntry.������).FirstOrDefault();

                            //        if (newEntry == null)
                            //        {
                            //            continue;
                            //        }

                            //        foreach (var property in typeof(EGRULSvLicense).GetProperties())
                            //        {
                            //            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                            //            {
                            //                if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                            //                    continue;

                            //                ChangeLog changes = new ChangeLog();

                            //                changes.������� = "EGRULSvLicense";
                            //                changes.������� = property.Name;
                            //                changes.���������� = property.GetValue(oldEntry)?.ToString();
                            //                changes.������������� = property.GetValue(newEntry)?.ToString();
                            //                changes.��� = documentXML.����.���;
                            //                changes.������������� = DateTime.Now;

                            //                //_dbcontext.����������������.Add(changes);
                            //            }
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_���������� = new List<EGRULSvLicense>();

                            //    foreach (var svLicense in documentXML.����.����������)
                            //    {
                            //        ULDB.�����_����������.Add(new EGRULSvLicense { Id = Guid.NewGuid().ToString(), ���������� = svLicense.����������, ������� = svLicense.�������, ������ = svLicense.������, �������������� = svLicense.��������������, ������������ = svLicense.������������, ������������ = svLicense.������������, ���������� = svLicense.�������.����������, ��� = svLicense.�������.���, id���� = ULDB.Id });
                            //    }
                            //}

                            break;

                        case "EGRULSvNaimUL":
                            if (documentXML.����.�������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvNaimUL previousEntry;
                                var newEntry = new EGRULSvNaimUL { Id = Guid.NewGuid().ToString(), ��� = documentXML.����.��������.�������.���, ���������� = documentXML.����.��������.�������.����������, ���������� = documentXML.����.��������.����������, ���������� = documentXML.����.��������.������������?.��������, id���� = ULDB.Id };

                                lock (listLockers[6])
                                {
                                    previousEntry = ULSubTables.�����_��������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[6])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.��������.�������.����������)
                                        {
                                            ULSubTables.�����_��������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_��������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_��������).Load();

                                        if (ULDB.�����_��������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_��������?.FirstOrDefault();
                                            ULSubTables.�����_��������_Delete.Add(ULDB.�����_��������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[6])
                                    {
                                        ULSubTables.�����_��������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.��������.�������.����������)
                                {
                                    foreach (var property in typeof(EGRULSvNaimUL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.������� = "EGRULSvNaimUL";
                                            changes.������� = property.Name;
                                            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                            changes.������������� = property.GetValue(newEntry)?.ToString();
                                            changes.��� = documentXML.����.���;
                                            changes.������������� = DateTime.Now;

                                            IPSubTables.����������������_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_��������).Load();

                            //if (ULDB.�����_��������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_��������.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_��������.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_�������� = new List<EGRULSvNaimUL> { new EGRULSvNaimUL { Id = ULDB.�����_��������.FirstOrDefault()?.Id != null ? ULDB.�����_��������.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ��� = documentXML.����.��������.�������.���, ���������� = documentXML.����.��������.�������.����������, ���������� = documentXML.����.��������.����������, ���������� = documentXML.����.��������.������������?.��������, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_��������.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvNaimUL).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_��������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvNaimUL";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_��������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_�������� = new List<EGRULSvNaimUL> { new EGRULSvNaimUL { Id = Guid.NewGuid().ToString(), ��� = documentXML.����.��������.�������.���, ���������� = documentXML.����.��������.�������.����������, ���������� = documentXML.����.��������.����������, ���������� = documentXML.����.��������.������������?.��������, id���� = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvObrUL":
                            if (documentXML.����.������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvObrUL previousEntry;
                                var newEntry = new EGRULSvObrUL { Id = Guid.NewGuid().ToString(), �������� = documentXML.����.�������.��������, ���� = documentXML.����.�������.����, ����������� = documentXML.����.�������.�������?.�����������, ���������� = documentXML.����.�������.�������?.����������, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.���, ������� = documentXML.����.�������.�������, ������ = documentXML.����.�������.������, ������ = documentXML.����.�������.������, id���� = ULDB.Id };

                                lock (listLockers[7])
                                {
                                    previousEntry = ULSubTables.�����_�������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[7])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.�������.�������.����������)
                                        {
                                            ULSubTables.�����_�������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_�������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_�������).Load();

                                        if (ULDB.�����_�������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_�������?.FirstOrDefault();
                                            ULSubTables.�����_�������_Delete.Add(ULDB.�����_�������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[7])
                                    {
                                        ULSubTables.�����_�������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.�������.�������.����������)
                                {
                                    foreach (var property in typeof(EGRULSvObrUL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.������� = "EGRULSvObrUL";
                                            changes.������� = property.Name;
                                            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                            changes.������������� = property.GetValue(newEntry)?.ToString();
                                            changes.��� = documentXML.����.���;
                                            changes.������������� = DateTime.Now;

                                            IPSubTables.����������������_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_�������).Load();

                            //if (ULDB.�����_�������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_�������.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_�������.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_������� = new List<EGRULSvObrUL> { new EGRULSvObrUL { Id = ULDB.�����_�������.FirstOrDefault()?.Id != null ? ULDB.�����_�������.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), �������� = documentXML.����.�������.��������, ���� = documentXML.����.�������.����, ����������� = documentXML.����.�������.�������?.�����������, ���������� = documentXML.����.�������.�������?.����������, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.���, ������� = documentXML.����.�������.�������, ������ = documentXML.����.�������.������, ������ = documentXML.����.�������.������, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_�������.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvObrUL).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_�������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvObrUL";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_�������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_������� = new List<EGRULSvObrUL> { new EGRULSvObrUL { Id = Guid.NewGuid().ToString(), �������� = documentXML.����.�������.��������, ���� = documentXML.����.�������.����, ����������� = documentXML.����.�������.�������?.�����������, ���������� = documentXML.����.�������.�������?.����������, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.���, ������� = documentXML.����.�������.�������, ������ = documentXML.����.�������.������, ������ = documentXML.����.�������.������, id���� = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvPodrazd":
                            if (documentXML.����.��������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                List<EGRULSvPodrazd> previousEntries;
                                var currentEntries = new List<EGRULSvPodrazd>();
                                var entriesDates = documentXML.����.���������.��������.Select(d => d.�������?.�������.����������).ToArray();
                                DateTime? currentEntriesDate = entriesDates.Count() > 0 ? entriesDates.Max() : DateTime.MinValue;

                                lock (listLockers[8])
                                {
                                    previousEntries = ULSubTables.�����_���������_Insert.Where(e => e.id���� == ULDB.Id).ToList();
                                }
                                if (previousEntries.Count != 0)
                                {
                                    lock (listLockers[8])
                                    {
                                        if (previousEntries.Select(d => d.����������).ToArray()?.Max() <= currentEntriesDate)
                                        {
                                            ULSubTables.�����_���������_Insert.RemoveAll(e => e.id���� == ULDB.Id);

                                            foreach (var svFilial in documentXML.����.���������.��������)
                                            {
                                                ULSubTables.�����_���������_Insert.Add(new EGRULSvPodrazd { Id = Guid.NewGuid().ToString(), �������� = svFilial.������?.��������, ��� = svFilial.�������?.���, ����������� = svFilial.�������?.�����������, ��������� = svFilial.�������?.���������, ������ = svFilial.�������?.������, ���������� = svFilial.�������?.������?.����������, ��������� = svFilial.�������?.�����?.���������, ��������� = svFilial.�������?.�����?.���������, �������� = svFilial.�������?.�����?.��������, ��������� = svFilial.�������?.������?.���������, �������� = svFilial.�������?.�����?.��������, ���������� = svFilial.�������?.�������.����������, ��� = svFilial.�������?.�������.���, id���� = ULDB.Id });
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_���������).Load();

                                        if (ULDB.�����_���������?.FirstOrDefault() != null)
                                        {
                                            previousEntries = ULDB.�����_���������.ToList();
                                            ULSubTables.�����_���������_Delete.Add(ULDB.�����_���������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[8])
                                    {
                                        foreach (var svFilial in documentXML.����.���������.��������)
                                        {
                                            ULSubTables.�����_���������_Insert.Add(new EGRULSvPodrazd { Id = Guid.NewGuid().ToString(), �������� = svFilial.������?.��������, ��� = svFilial.�������?.���, ����������� = svFilial.�������?.�����������, ��������� = svFilial.�������?.���������, ������ = svFilial.�������?.������, ���������� = svFilial.�������?.������?.����������, ��������� = svFilial.�������?.�����?.���������, ��������� = svFilial.�������?.�����?.���������, �������� = svFilial.�������?.�����?.��������, ��������� = svFilial.�������?.������?.���������, �������� = svFilial.�������?.�����?.��������, ���������� = svFilial.�������?.�������.����������, ��� = svFilial.�������?.�������.���, id���� = ULDB.Id });
                                        }
                                    }
                                }

                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_���������).Load();

                            //if (ULDB.�����_���������?.Count > 0)
                            //{
                            //    foreach (var entry in ULDB.�����_���������)
                            //    {
                            //        _dbcontext.Entry(entry).State = EntityState.Deleted;
                            //    }

                            //    //var previousEntries = ULDB.�����_���������;

                            //    ULDB.�����_��������� = new List<EGRULSvPodrazd>();

                            //    foreach (var svFilial in documentXML.����.���������.��������)
                            //    {
                            //        ULDB.�����_���������.Add(new EGRULSvPodrazd { Id = Guid.NewGuid().ToString(), �������� = svFilial.������?.��������, ��� = svFilial.�������?.���, ����������� = svFilial.�������?.�����������, ��������� = svFilial.�������?.���������, ������ = svFilial.�������?.������, ���������� = svFilial.�������?.������?.����������, ��������� = svFilial.�������?.�����?.���������, ��������� = svFilial.�������?.�����?.���������, �������� = svFilial.�������?.�����?.��������, ��������� = svFilial.�������?.������?.���������, �������� = svFilial.�������?.�����?.��������, ���������� = svFilial.�������?.�������.����������, ��� = svFilial.�������?.�������.���, id���� = ULDB.Id });
                            //        _dbcontext.Entry(ULDB.�����_���������.Last()).State = EntityState.Added;
                            //    }

                            //    //var currentEntries = ULDB.�����_���������;

                            //    //foreach (var oldEntry in previousEntries)
                            //    //{
                            //    //    var newEntry = currentEntries.Where(e => e.������ == oldEntry.������).FirstOrDefault();

                            //    //    if (newEntry == null)
                            //    //    {
                            //    //        continue;
                            //    //    }

                            //    //    foreach (var property in typeof(EGRULSvLicense).GetProperties())
                            //    //    {
                            //    //        if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                            //    //        {
                            //    //            if (property.Name == "Id" || property.Name == "id����" || property.Name == "��")
                            //    //                continue;

                            //    //            ChangeLog changes = new ChangeLog();

                            //    //            changes.������� = "EGRULSvLicense";
                            //    //            changes.������� = property.Name;
                            //    //            changes.���������� = property.GetValue(oldEntry)?.ToString();
                            //    //            changes.������������� = property.GetValue(newEntry)?.ToString();
                            //    //            changes.��� = documentXML.����.���;
                            //    //            changes.������������� = DateTime.Now;

                            //    //            //_dbcontext.����������������.Add(changes);
                            //    //        }
                            //    //    }
                            //    //}
                            //}
                            //else
                            //{
                            //    ULDB.�����_��������� = new List<EGRULSvPodrazd>();

                            //    foreach (var svFilial in documentXML.����.���������.��������)
                            //    {
                            //        ULDB.�����_���������.Add(new EGRULSvPodrazd { Id = Guid.NewGuid().ToString(), �������� = svFilial.������?.��������, ��� = svFilial.�������?.���, ����������� = svFilial.�������?.�����������, ��������� = svFilial.�������?.���������, ������ = svFilial.�������?.������, ���������� = svFilial.�������?.������?.����������, ��������� = svFilial.�������?.�����?.���������, ��������� = svFilial.�������?.�����?.���������, �������� = svFilial.�������?.�����?.��������, ��������� = svFilial.�������?.������?.���������, �������� = svFilial.�������?.�����?.��������, ���������� = svFilial.�������?.�������.����������, ��� = svFilial.�������?.�������.���, id���� = ULDB.Id });
                            //    }
                            //}

                            break;

                        case "EGRULSvPredsh":
                            if (documentXML.����.������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvPredsh previousEntry;
                                var newEntry = new EGRULSvPredsh { Id = Guid.NewGuid().ToString(), ��� = documentXML.����.�������.���, ���� = documentXML.����.�������.����, ���������� = documentXML.����.�������.����������, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.���, id���� = ULDB.Id };

                                lock (listLockers[9])
                                {
                                    previousEntry = ULSubTables.�����_�������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[9])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.�������.�������.����������)
                                        {
                                            ULSubTables.�����_�������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_�������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_�������).Load();

                                        if (ULDB.�����_�������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_�������?.FirstOrDefault();
                                            ULSubTables.�����_�������_Delete.Add(ULDB.�����_�������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[9])
                                    {
                                        ULSubTables.�����_�������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.�������.�������.����������)
                                {
                                    foreach (var property in typeof(EGRULSvPredsh).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.������� = "EGRULSvPredsh";
                                            changes.������� = property.Name;
                                            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                            changes.������������� = property.GetValue(newEntry)?.ToString();
                                            changes.��� = documentXML.����.���;
                                            changes.������������� = DateTime.Now;

                                            IPSubTables.����������������_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_�������).Load();

                            //if (ULDB.�����_�������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_�������.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_�������.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_������� = new List<EGRULSvPredsh> { new EGRULSvPredsh { Id = ULDB.�����_�������.FirstOrDefault()?.Id != null ? ULDB.�����_�������.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ��� = documentXML.����.�������.���, ���� = documentXML.����.�������.����, ���������� = documentXML.����.�������.����������, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.���, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_�������.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvPredsh).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_�������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvPredsh";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_�������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_������� = new List<EGRULSvPredsh> { new EGRULSvPredsh { Id = Guid.NewGuid().ToString(), ��� = documentXML.����.�������.���, ���� = documentXML.����.�������.����, ���������� = documentXML.����.�������.����������, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.���, id���� = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvPreem":
                            if (documentXML.����.������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvPreem previousEntry;
                                var newEntry = new EGRULSvPreem { Id = Guid.NewGuid().ToString(), ��� = documentXML.����.�������.���, ���� = documentXML.����.�������.����, ���������� = documentXML.����.�������.����������, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.���, id���� = ULDB.Id };

                                lock (listLockers[10])
                                {
                                    previousEntry = ULSubTables.�����_�������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[10])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.�������.�������.����������)
                                        {
                                            ULSubTables.�����_�������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_�������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_�������).Load();

                                        if (ULDB.�����_�������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_�������?.FirstOrDefault();
                                            ULSubTables.�����_�������_Delete.Add(ULDB.�����_�������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[10])
                                    {
                                        ULSubTables.�����_�������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.�������.�������.����������)
                                {
                                    foreach (var property in typeof(EGRULSvPreem).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.������� = "EGRULSvPreem";
                                            changes.������� = property.Name;
                                            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                            changes.������������� = property.GetValue(newEntry)?.ToString();
                                            changes.��� = documentXML.����.���;
                                            changes.������������� = DateTime.Now;

                                            IPSubTables.����������������_Insert.Add(changes);
                                        }
                                    }
                                }
                            }
                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_�������).Load();

                            //if (ULDB.�����_�������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_�������.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_�������.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_������� = new List<EGRULSvPreem> { new EGRULSvPreem { Id = ULDB.�����_�������.FirstOrDefault()?.Id != null ? ULDB.�����_�������.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ��� = documentXML.����.�������.���, ���� = documentXML.����.�������.����, ���������� = documentXML.����.�������.����������, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.���, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_�������.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvPreem).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_�������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvPreem";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_�������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_������� = new List<EGRULSvPreem> { new EGRULSvPreem { Id = Guid.NewGuid().ToString(), ��� = documentXML.����.�������.���, ���� = documentXML.����.�������.����, ���������� = documentXML.����.�������.����������, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.���, id���� = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvPrekrUL":
                            if (documentXML.����.��������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvPrekrUL previousEntry;
                                var newEntry = new EGRULSvPrekrUL { Id = Guid.NewGuid().ToString(), ����������� = documentXML.����.���������.���������?.�����������, ����� = documentXML.����.���������.��������?.�����, ������ = documentXML.����.���������.��������?.������, ��� = documentXML.����.���������.�������.���, ���������� = documentXML.����.���������.�������.����������, id���� = ULDB.Id };

                                lock (listLockers[11])
                                {
                                    previousEntry = ULSubTables.�����_���������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[11])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.���������.�������.����������)
                                        {
                                            ULSubTables.�����_���������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_���������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_���������).Load();

                                        if (ULDB.�����_���������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_���������?.FirstOrDefault();
                                            ULSubTables.�����_���������_Delete.Add(ULDB.�����_���������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[11])
                                    {
                                        ULSubTables.�����_���������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.���������.�������.����������)
                                {
                                    foreach (var property in typeof(EGRULSvPrekrUL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.������� = "EGRULSvPrekrUL";
                                            changes.������� = property.Name;
                                            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                            changes.������������� = property.GetValue(newEntry)?.ToString();
                                            changes.��� = documentXML.����.���;
                                            changes.������������� = DateTime.Now;

                                            IPSubTables.����������������_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_���������).Load();

                            //if (ULDB.�����_���������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_���������.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_���������.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_��������� = new List<EGRULSvPrekrUL> { new EGRULSvPrekrUL { Id = ULDB.�����_���������.FirstOrDefault()?.Id != null ? ULDB.�����_���������.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ����������� = documentXML.����.���������.���������?.�����������, ����� = documentXML.����.���������.��������?.�����, ������ = documentXML.����.���������.��������?.������, ��� = documentXML.����.���������.�������.���, ���������� = documentXML.����.���������.�������.����������, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_���������.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvPrekrUL).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_���������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvPrekrUL";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_���������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_��������� = new List<EGRULSvPrekrUL> { new EGRULSvPrekrUL { Id = Guid.NewGuid().ToString(), ����������� = documentXML.����.���������.���������?.�����������, ����� = documentXML.����.���������.��������?.�����, ������ = documentXML.����.���������.��������?.������, ��� = documentXML.����.���������.�������.���, ���������� = documentXML.����.���������.�������.����������, id���� = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvRegOrg":
                            if (documentXML.����.�������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvRegOrg previousEntry;
                                var newEntry = new EGRULSvRegOrg { Id = Guid.NewGuid().ToString(), ����� = documentXML.����.��������?.�����, ������ = documentXML.����.��������?.������, ����� = documentXML.����.��������?.�����, ���������� = documentXML.����.��������?.�������.����������, ��� = documentXML.����.��������?.�������.���, id���� = ULDB.Id };

                                lock (listLockers[12])
                                {
                                    previousEntry = ULSubTables.�����_��������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[12])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.��������.�������.����������)
                                        {
                                            ULSubTables.�����_��������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_��������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_��������).Load();

                                        if (ULDB.�����_��������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_��������?.FirstOrDefault();
                                            ULSubTables.�����_��������_Delete.Add(ULDB.�����_��������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[12])
                                    {
                                        ULSubTables.�����_��������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.��������.�������.����������)
                                {
                                    foreach (var property in typeof(EGRULSvRegOrg).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.������� = "EGRULSvRegOrg";
                                            changes.������� = property.Name;
                                            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                            changes.������������� = property.GetValue(newEntry)?.ToString();
                                            changes.��� = documentXML.����.���;
                                            changes.������������� = DateTime.Now;

                                            IPSubTables.����������������_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_��������).Load();

                            //if (ULDB.�����_��������?.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_��������?.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_��������?.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_�������� = new List<EGRULSvRegOrg> { new EGRULSvRegOrg { Id = ULDB.�����_��������?.FirstOrDefault()?.Id != null ? ULDB.�����_��������?.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ����� = documentXML.����.��������?.�����, ������ = documentXML.����.��������?.������, ����� = documentXML.����.��������?.�����, ���������� = documentXML.����.��������?.�������.����������, ��� = documentXML.����.��������?.�������.���, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_��������?.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvRegOrg).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_��������?.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvRegOrg";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_��������?.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_�������� = new List<EGRULSvRegOrg> { new EGRULSvRegOrg { Id = Guid.NewGuid().ToString(), ����� = documentXML.����.��������?.�����, ������ = documentXML.����.��������?.������, ����� = documentXML.����.��������?.�����, ���������� = documentXML.����.��������?.�������.����������, ��� = documentXML.����.��������?.�������.���, id���� = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvRegPF":
                            if (documentXML.����.������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvRegPF previousEntry;
                                var newEntry = new EGRULSvRegPF { Id = Guid.NewGuid().ToString(), ������� = documentXML.����.�������.�������, �������� = documentXML.����.�������.��������, ������ = documentXML.����.�������.�������?.������, ����� = documentXML.����.�������.�������?.�����, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.���, id���� = ULDB.Id };

                                lock (listLockers[13])
                                {
                                    previousEntry = ULSubTables.�����_�������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[13])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.�������.�������.����������)
                                        {
                                            ULSubTables.�����_�������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_�������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_�������).Load();

                                        if (ULDB.�����_�������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_�������?.FirstOrDefault();
                                            ULSubTables.�����_�������_Delete.Add(ULDB.�����_�������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[13])
                                    {
                                        ULSubTables.�����_�������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.�������.�������.����������)
                                {
                                    foreach (var property in typeof(EGRULSvRegPF).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.������� = "EGRULSvRegPF";
                                            changes.������� = property.Name;
                                            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                            changes.������������� = property.GetValue(newEntry)?.ToString();
                                            changes.��� = documentXML.����.���;
                                            changes.������������� = DateTime.Now;

                                            IPSubTables.����������������_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_�������).Load();

                            //if (ULDB.�����_�������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_�������.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_�������.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_������� = new List<EGRULSvRegPF> { new EGRULSvRegPF { Id = ULDB.�����_�������.FirstOrDefault()?.Id != null ? ULDB.�����_�������.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ������� = documentXML.����.�������.�������, �������� = documentXML.����.�������.��������, ������ = documentXML.����.�������.�������?.������, ����� = documentXML.����.�������.�������?.�����, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.���, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_�������.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvRegPF).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_�������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvRegPF";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_�������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_������� = new List<EGRULSvRegPF> { new EGRULSvRegPF { Id = Guid.NewGuid().ToString(), ������� = documentXML.����.�������.�������, �������� = documentXML.����.�������.��������, ������ = documentXML.����.�������.�������?.������, ����� = documentXML.����.�������.�������?.�����, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.���, id���� = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvRegFSS":
                            if (documentXML.����.�������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvRegFSS previousEntry;
                                var newEntry = new EGRULSvRegFSS { Id = Guid.NewGuid().ToString(), ������� = documentXML.����.��������?.�������, ��������� = documentXML.����.��������?.���������, ������� = documentXML.����.��������?.��������?.�������, ������ = documentXML.����.��������?.��������?.������, ���������� = documentXML.����.��������?.�������.����������, ��� = documentXML.����.��������?.�������.���, id���� = ULDB.Id };

                                lock (listLockers[14])
                                {
                                    previousEntry = ULSubTables.�����_��������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[14])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.��������.�������.����������)
                                        {
                                            ULSubTables.�����_��������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_��������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_��������).Load();

                                        if (ULDB.�����_��������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_��������?.FirstOrDefault();
                                            ULSubTables.�����_��������_Delete.Add(ULDB.�����_��������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[14])
                                    {
                                        ULSubTables.�����_��������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.��������.�������.����������)
                                {
                                    foreach (var property in typeof(EGRULSvRegFSS).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.������� = "EGRULSvRegFSS";
                                            changes.������� = property.Name;
                                            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                            changes.������������� = property.GetValue(newEntry)?.ToString();
                                            changes.��� = documentXML.����.���;
                                            changes.������������� = DateTime.Now;

                                            IPSubTables.����������������_Insert.Add(changes);
                                        }
                                    }
                                }
                            }
                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_��������).Load();

                            //if (ULDB.�����_��������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_��������.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_��������.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_�������� = new List<EGRULSvRegFSS> { new EGRULSvRegFSS { Id = ULDB.�����_��������.FirstOrDefault()?.Id != null ? ULDB.�����_��������.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ������� = documentXML.����.��������?.�������, ��������� = documentXML.����.��������?.���������, ������� = documentXML.����.��������?.��������?.�������, ������ = documentXML.����.��������?.��������?.������, ���������� = documentXML.����.��������?.�������.����������, ��� = documentXML.����.��������?.�������.���, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_��������.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvRegFSS).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_��������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvRegFSS";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_��������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_�������� = new List<EGRULSvRegFSS> { new EGRULSvRegFSS { Id = Guid.NewGuid().ToString(), ������� = documentXML.����.��������.������� } };
                            //}

                            break;

                        case "EGRULSvReorg":
                            if (documentXML.����.������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvReorg previousEntry;
                                var newEntry = new EGRULSvReorg { Id = Guid.NewGuid().ToString(), ��� = documentXML.����.�������.���������?.���, ���� = documentXML.����.�������.���������?.����, ����������� = documentXML.����.�������.��������?.���������, ������������ = documentXML.����.�������.��������?.����������, ���������� = documentXML.����.�������.���������?.����������, ����������� = documentXML.����.�������.���������?.�����������, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.���, id���� = ULDB.Id };

                                lock (listLockers[15])
                                {
                                    previousEntry = ULSubTables.�����_�������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[15])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.�������.�������.����������)
                                        {
                                            ULSubTables.�����_�������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_�������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_�������).Load();

                                        if (ULDB.�����_�������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_�������?.FirstOrDefault();
                                            ULSubTables.�����_�������_Delete.Add(ULDB.�����_�������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[15])
                                    {
                                        ULSubTables.�����_�������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.�������.�������.����������)
                                {
                                    foreach (var property in typeof(EGRULSvReorg).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.������� = "EGRULSvReorg";
                                            changes.������� = property.Name;
                                            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                            changes.������������� = property.GetValue(newEntry)?.ToString();
                                            changes.��� = documentXML.����.���;
                                            changes.������������� = DateTime.Now;

                                            IPSubTables.����������������_Insert.Add(changes);
                                        }
                                    }
                                }
                            }
                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_�������).Load();

                            //if (ULDB.�����_�������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_�������.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_�������.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_������� = new List<EGRULSvReorg> { new EGRULSvReorg { Id = ULDB.�����_�������.FirstOrDefault()?.Id != null ? ULDB.�����_�������.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ��� = documentXML.����.�������.���������?.���, ���� = documentXML.����.�������.���������?.����, ����������� = documentXML.����.�������.��������?.���������, ������������ = documentXML.����.�������.��������?.����������, ���������� = documentXML.����.�������.���������?.����������, ����������� = documentXML.����.�������.���������?.�����������, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.��� } };
                            //    _dbcontext.Entry(ULDB.�����_�������.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvReorg).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_�������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvReorg";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_�������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_������� = new List<EGRULSvReorg> { new EGRULSvReorg { Id = Guid.NewGuid().ToString(), ��� = documentXML.����.�������.���������?.���, ���� = documentXML.����.�������.���������?.����, ����������� = documentXML.����.�������.��������?.���������, ������������ = documentXML.����.�������.��������?.����������, ���������� = documentXML.����.�������.���������?.����������, ����������� = documentXML.����.�������.���������?.�����������, ���������� = documentXML.����.�������.�������.����������, ��� = documentXML.����.�������.�������.��� } };
                            //}

                            break;

                        case "EGRULSvStatus":
                            if (documentXML.����.���������������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvStatus previousEntry;
                                var newEntry = new EGRULSvStatus { Id = Guid.NewGuid().ToString(), ������������ = documentXML.����.����������������.��������?.����������, ����������� = documentXML.����.����������������.��������?.���������, ������������ = documentXML.����.����������������.�����������?.������������, �������������� = documentXML.����.����������������.�����������?.��������������, �������� = documentXML.����.����������������.�����������?.��������, ������� = documentXML.����.����������������.�����������?.�������, ���������� = documentXML.����.����������������.�������.����������, ��� = documentXML.����.����������������.�������.���, id���� = ULDB.Id };

                                lock (listLockers[17])
                                {
                                    previousEntry = ULSubTables.�����_��������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[17])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.����������������.�������.����������)
                                        {
                                            ULSubTables.�����_��������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_��������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_��������).Load();

                                        if (ULDB.�����_��������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_��������?.FirstOrDefault();
                                            ULSubTables.�����_��������_Delete.Add(ULDB.�����_��������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[17])
                                    {
                                        ULSubTables.�����_��������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.����������������.�������.����������)
                                {
                                    foreach (var property in typeof(EGRULSvStatus).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.������� = "EGRULSvStatus";
                                            changes.������� = property.Name;
                                            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                            changes.������������� = property.GetValue(newEntry)?.ToString();
                                            changes.��� = documentXML.����.���;
                                            changes.������������� = DateTime.Now;

                                            IPSubTables.����������������_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_��������).Load();

                            //if (ULDB.�����_��������?.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_��������?.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_��������?.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_�������� = new List<EGRULSvStatus> { new EGRULSvStatus { Id = ULDB.�����_��������?.FirstOrDefault()?.Id != null ? ULDB.�����_��������?.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ������������ = documentXML.����.����������������.��������?.����������, ����������� = documentXML.����.����������������.��������?.���������, ������������ = documentXML.����.����������������.�����������?.������������, �������������� = documentXML.����.����������������.�����������?.��������������, �������� = documentXML.����.����������������.�����������?.��������, ������� = documentXML.����.����������������.�����������?.�������, ���������� = documentXML.����.����������������.�������.����������, ��� = documentXML.����.����������������.�������.���, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_��������?.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvStatus).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_��������?.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvStatus";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_��������?.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_�������� = new List<EGRULSvStatus> { new EGRULSvStatus { Id = Guid.NewGuid().ToString(), ������������ = documentXML.����.����������������.��������?.����������, ����������� = documentXML.����.����������������.��������?.���������, ������������ = documentXML.����.����������������.�����������?.������������, �������������� = documentXML.����.����������������.�����������?.��������������, �������� = documentXML.����.����������������.�����������?.��������, ������� = documentXML.����.����������������.�����������?.�������, ���������� = documentXML.����.����������������.�������.����������, ��� = documentXML.����.����������������.�������.���, id���� = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvUstKap":
                            if (documentXML.����.�������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvUstKap previousEntry;
                                var newEntry = new EGRULSvUstKap { Id = Guid.NewGuid().ToString(), ���������� = documentXML.����.��������.����������, ������ = documentXML.����.��������.������, ���������� = documentXML.����.��������.�������.����������, ��� = documentXML.����.��������.�������.���, id���� = ULDB.Id };

                                lock (listLockers[18])
                                {
                                    previousEntry = ULSubTables.�����_��������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[18])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.��������.�������.����������)
                                        {
                                            ULSubTables.�����_��������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_��������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_��������).Load();

                                        if (ULDB.�����_��������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_��������?.FirstOrDefault();
                                            ULSubTables.�����_��������_Delete.Add(ULDB.�����_��������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[18])
                                    {
                                        ULSubTables.�����_��������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.��������.�������.����������)
                                {
                                    foreach (var property in typeof(EGRULSvUstKap).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.������� = "EGRULSvUstKap";
                                            changes.������� = property.Name;
                                            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                            changes.������������� = property.GetValue(newEntry)?.ToString();
                                            changes.��� = documentXML.����.���;
                                            changes.������������� = DateTime.Now;

                                            IPSubTables.����������������_Insert.Add(changes);
                                        }
                                    }
                                }
                            }
                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_��������).Load();

                            //if (ULDB.�����_��������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_��������.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_��������.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_�������� = new List<EGRULSvUstKap> { new EGRULSvUstKap { Id = ULDB.�����_��������.FirstOrDefault()?.Id != null ? ULDB.�����_��������.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ���������� = documentXML.����.��������.����������, ������ = documentXML.����.��������.������, ���������� = documentXML.����.��������.�������.����������, ��� = documentXML.����.��������.�������.���, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_��������.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvUstKap).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_��������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvUstKap";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_��������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_�������� = new List<EGRULSvUstKap> { new EGRULSvUstKap { Id = Guid.NewGuid().ToString(), ���������� = documentXML.����.��������.����������, ������ = documentXML.����.��������.������, ���������� = documentXML.����.��������.�������.����������, ��� = documentXML.����.��������.�������.���, id���� = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvFounder":
                            if (documentXML.����.��������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                List<EGRULSvFounder> previousEntries;
                                var currentEntries = new List<EGRULSvFounder>();
                                var founderDates = new[] { documentXML.����.���������.�����.Where(x => x.����������� != null).Select(d => d.�����������.����������), documentXML.����.���������.��������.Where(x => x.����������� != null).Select(d => d.�����������.����������) }.SelectMany(date => date).ToArray();
                                DateTime? currentEntriesDate = founderDates.Count() > 0 ? founderDates.Max() : DateTime.MinValue;

                                lock (listLockers[19])
                                {
                                    previousEntries = ULSubTables.�����_���������_Insert.Where(e => e.id���� == ULDB.Id).ToList();

                                    if (previousEntries.Count != 0)
                                    {
                                        if (previousEntries.Select(d => d.����������).ToArray()?.Max() <= currentEntriesDate)
                                        {
                                            ULSubTables.�����_���������_Insert.RemoveAll(e => e.id���� == ULDB.Id);

                                            foreach (var svFL in documentXML.����.���������.�����)
                                            {
                                                ULSubTables.�����_���������_Insert.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ���������� = svFL.�����������?.����������, ��� = svFL.�����������?.���, �������� = svFL.����?.��������, ��� = svFL.����?.���, ������� = svFL.����?.�������, ����� = svFL.����?.�����, ���������� = svFL.����������?.����������, ���������� = svFL.����������?.����������?.�������, id���� = ULDB.Id });
                                            }

                                            foreach (var svUL in documentXML.����.���������.��������)
                                            {
                                                ULSubTables.�����_���������_Insert.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ���������� = svUL.�����������?.����������, ��� = svUL.�����������?.���, ��� = svUL.���������?.���, ���� = svUL.���������?.����, ���������� = svUL.���������?.����������, �������������� = svUL.�����������?.��������������, �������������� = svUL.�����������?.��������������, ���������� = svUL.����������?.����������, ���������� = svUL.����������?.����������?.�������, id���� = ULDB.Id });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (firstTime == false)
                                        {
                                            _dbcontext.Entry(ULDB).Collection(u => u.�����_���������).Load();

                                            if (ULDB.�����_���������?.FirstOrDefault() != null)
                                            {
                                                previousEntries = ULDB.�����_���������.ToList();
                                                ULSubTables.�����_���������_Delete.Add(ULDB.�����_���������?.FirstOrDefault().id����.ToString());
                                            }
                                        }

                                        foreach (var svFL in documentXML.����.���������.�����)
                                        {
                                            ULSubTables.�����_���������_Insert.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ���������� = svFL.�����������?.����������, ��� = svFL.�����������?.���, �������� = svFL.����?.��������, ��� = svFL.����?.���, ������� = svFL.����?.�������, ����� = svFL.����?.�����, ���������� = svFL.����������?.����������, ���������� = svFL.����������?.����������?.�������, id���� = ULDB.Id });
                                        }

                                        foreach (var svUL in documentXML.����.���������.��������)
                                        {
                                            ULSubTables.�����_���������_Insert.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ���������� = svUL.�����������?.����������, ��� = svUL.�����������?.���, ��� = svUL.���������?.���, ���� = svUL.���������?.����, ���������� = svUL.���������?.����������, �������������� = svUL.�����������?.��������������, �������������� = svUL.�����������?.��������������, ���������� = svUL.����������?.����������, ���������� = svUL.����������?.����������?.�������, id���� = ULDB.Id });
                                        }
                                    }
                                }

                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_���������).Load();

                            //if (ULDB.�����_���������?.Count > 0)
                            //{
                            //    foreach (var entry in ULDB.�����_���������)
                            //    {
                            //        _dbcontext.Entry(entry).State = EntityState.Deleted;
                            //    }

                            //    var previousEntries = ULDB.�����_���������;

                            //    ULDB.�����_��������� = new List<EGRULSvFounder>();

                            //    foreach (var svFL in documentXML.����.���������.�����)
                            //    {
                            //        ULDB.�����_���������.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ���������� = svFL.�����������?.����������, ��� = svFL.�����������?.���, �������� = svFL.����?.��������, ��� = svFL.����?.���, ������� = svFL.����?.�������, ����� = svFL.����?.�����, ���������� = svFL.����������?.����������, ���������� = svFL.����������?.����������?.�������, id���� = ULDB.Id });
                            //        _dbcontext.Entry(ULDB.�����_���������.Last()).State = EntityState.Added;
                            //    }

                            //    foreach (var svUL in documentXML.����.���������.��������)
                            //    {
                            //        ULDB.�����_���������.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ���������� = svUL.�����������?.����������, ��� = svUL.�����������?.���, ��� = svUL.���������?.���, ���� = svUL.���������?.����, ���������� = svUL.���������?.����������, �������������� = svUL.�����������?.��������������, �������������� = svUL.�����������?.��������������, ���������� = svUL.����������?.����������, ���������� = svUL.����������?.����������?.�������, id���� = ULDB.Id });
                            //        _dbcontext.Entry(ULDB.�����_���������.Last()).State = EntityState.Added;
                            //    }

                            //    var currentEntries = ULDB.�����_���������;

                            //    foreach (var oldEntry in previousEntries)
                            //    {
                            //        var newEntry = currentEntries.Where(e => e.����� == oldEntry.����� && e.����� != null || e.��� == oldEntry.��� && e.��� != null).FirstOrDefault();

                            //        if (newEntry == null)
                            //        {
                            //            continue;
                            //        }

                            //        foreach (var property in typeof(EGRULSvFounder).GetProperties())
                            //        {
                            //            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                            //            {
                            //                if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                            //                    continue;

                            //                ChangeLog changes = new ChangeLog();

                            //                changes.������� = "EGRULSvFounder";
                            //                changes.������� = property.Name;
                            //                changes.���������� = property.GetValue(oldEntry)?.ToString();
                            //                changes.������������� = property.GetValue(newEntry)?.ToString();
                            //                changes.��� = documentXML.����.���;
                            //                changes.������������� = DateTime.Now;

                            //                //_dbcontext.����������������.Add(changes);
                            //            }
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_��������� = new List<EGRULSvFounder>();

                            //    foreach (var svFL in documentXML.����.���������.�����)
                            //    {
                            //        ULDB.�����_���������.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ���������� = svFL.�����������?.����������, ��� = svFL.�����������?.���, �������� = svFL.����?.��������, ��� = svFL.����?.���, ������� = svFL.����?.�������, ����� = svFL.����?.�����, ���������� = svFL.����������?.����������, ���������� = svFL.����������?.����������?.�������, id���� = ULDB.Id });
                            //    }

                            //    foreach (var svUL in documentXML.����.���������.��������)
                            //    {
                            //        ULDB.�����_���������.Add(new EGRULSvFounder { Id = Guid.NewGuid().ToString(), ���������� = svUL.�����������?.����������, ��� = svUL.�����������?.���, ��� = svUL.���������?.���, ���� = svUL.���������?.����, ���������� = svUL.���������?.����������, �������������� = svUL.�����������?.��������������, �������������� = svUL.�����������?.��������������, ���������� = svUL.����������?.����������, ���������� = svUL.����������?.����������?.�������, id���� = ULDB.Id });
                            //    }
                            //}

                            break;

                        case "EGRULSvUL":
                            if (documentXML.���� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvUL previousEntry;
                                var newEntry = new EGRULSvUL { Id = Guid.NewGuid().ToString(), ����������� = documentXML.����.�����������, ������ = documentXML.����.������, ������ = documentXML.����.������, ��� = documentXML.����.���, ��� = documentXML.����.���, �������� = documentXML.����.��������, ���� = documentXML.����.����, ������� = documentXML.����.�������, �������� = (String.IsNullOrEmpty(documentXML.����.��������.������������?.��������)) ? documentXML.����.��������.���������� : documentXML.����.��������.������������?.��������, �������� = documentXML.����.�������?.����������?.��������, id���� = ULDB.Id };

                                lock (listLockers[20])
                                {
                                    previousEntry = ULSubTables.�����_����_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[20])
                                    {
                                        if (previousEntry.������� <= documentXML.����.�������)
                                        {
                                            ULSubTables.�����_����_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_����_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_����).Load();

                                        if (ULDB.�����_����?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_����?.FirstOrDefault();
                                            ULSubTables.�����_����_Delete.Add(ULDB.�����_����?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[20])
                                    {
                                        ULSubTables.�����_����_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.������� <= documentXML.����.�������)
                                {
                                    foreach (var property in typeof(EGRULSvUL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.������� = "EGRULSvUL";
                                            changes.������� = property.Name;
                                            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                            changes.������������� = property.GetValue(newEntry)?.ToString();
                                            changes.��� = documentXML.����.���;
                                            changes.������������� = DateTime.Now;

                                            IPSubTables.����������������_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_����).Load();

                            //if (ULDB.�����_����.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_����.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_����.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_���� = new List<EGRULSvUL> { new EGRULSvUL { Id = ULDB.�����_����.FirstOrDefault()?.Id != null ? ULDB.�����_����.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ����������� = documentXML.����.�����������, ������ = documentXML.����.������, ������ = documentXML.����.������, ��� = documentXML.����.���, ��� = documentXML.����.���, �������� = documentXML.����.��������, ���� = documentXML.����.����, ������� = documentXML.����.�������, �������� = (String.IsNullOrEmpty(documentXML.����.��������.������������?.��������)) ? documentXML.����.��������.���������� : documentXML.����.��������.������������?.��������, �������� = documentXML.����.�������?.����������?.��������, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_����.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvUL).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_����.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvUL";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_����.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_���� = new List<EGRULSvUL> { new EGRULSvUL { Id = Guid.NewGuid().ToString(), ����������� = documentXML.����.�����������, ������ = documentXML.����.������, ������ = documentXML.����.������, ��� = documentXML.����.���, ��� = documentXML.����.���, �������� = documentXML.����.��������, ���� = documentXML.����.����, ������� = documentXML.����.�������, �������� = (String.IsNullOrEmpty(documentXML.����.��������.������������?.��������)) ? documentXML.����.��������.���������� : documentXML.����.��������.������������?.��������, �������� = documentXML.����.�������?.����������?.��������, id���� = ULDB.Id } };
                            //}

                            break;

                        case "EGRULSvedDoljnFL":
                            if (documentXML.����.����������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRULSvedDoljnFL previousEntry;
                                var newEntry = new EGRULSvedDoljnFL { Id = Guid.NewGuid().ToString(), ���������� = documentXML.����.�����������.�����������?.����������, ��� = documentXML.����.�����������.�����������?.���, �������� = documentXML.����.�����������.����?.��������, ��� = documentXML.����.�����������.����?.���, ������� = documentXML.����.�����������.����?.�������, ��������� = documentXML.����.�����������.�������?.���������, ������������ = documentXML.����.�����������.�������?.������������, ����� = documentXML.����.�����������.����?.�����, �������� = documentXML.����.�����������.�������?.��������, id���� = ULDB.Id };

                                lock (listLockers[21])
                                {
                                    previousEntry = ULSubTables.�����_�����������_Insert.Where(e => e.id���� == ULDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[21])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.�����������.�����������?.����������)
                                        {
                                            ULSubTables.�����_�����������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            ULSubTables.�����_�����������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(ULDB).Collection(u => u.�����_�����������).Load();

                                        if (ULDB.�����_�����������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = ULDB.�����_�����������?.FirstOrDefault();
                                            ULSubTables.�����_�����������_Delete.Add(ULDB.�����_�����������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[21])
                                    {
                                        ULSubTables.�����_�����������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.�����������.�����������?.����������)
                                {
                                    foreach (var property in typeof(EGRULSvedDoljnFL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            if (property.Name == "Id" || property.Name == "id����" || property.Name == "������")
                                                continue;

                                            ChangeLog changes = new ChangeLog();

                                            changes.������� = "EGRULSvedDoljnFL";
                                            changes.������� = property.Name;
                                            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                            changes.������������� = property.GetValue(newEntry)?.ToString();
                                            changes.��� = documentXML.����.���;
                                            changes.������������� = DateTime.Now;

                                            IPSubTables.����������������_Insert.Add(changes);
                                        }
                                    }
                                }
                            }

                            //_dbcontext.Entry(ULDB).Collection(u => u.�����_�����������).Load();

                            //if (ULDB.�����_�����������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = ULDB.�����_�����������.FirstOrDefault();

                            //    _dbcontext.Entry(ULDB.�����_�����������.FirstOrDefault()).State = EntityState.Deleted;
                            //    ULDB.�����_����������� = new List<EGRULSvedDoljnFL> { new EGRULSvedDoljnFL { Id = ULDB.�����_�����������.FirstOrDefault()?.Id != null ? ULDB.�����_�����������.FirstOrDefault()?.Id : Guid.NewGuid().ToString(), ���������� = documentXML.����.�����������.�����������?.����������, ��� = documentXML.����.�����������.�����������?.���, �������� = documentXML.����.�����������.����?.��������, ��� = documentXML.����.�����������.����?.���, ������� = documentXML.����.�����������.����?.�������, ��������� = documentXML.����.�����������.�������?.���������, ������������ = documentXML.����.�����������.�������?.������������, ����� = documentXML.����.�����������.����?.�����, �������� = documentXML.����.�����������.�������?.��������, id���� = ULDB.Id } };
                            //    _dbcontext.Entry(ULDB.�����_�����������.FirstOrDefault()).State = EntityState.Added;

                            //    foreach (var property in typeof(EGRULSvedDoljnFL).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(ULDB.�����_�����������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRULSvedDoljnFL";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(ULDB.�����_�����������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.���;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    ULDB.�����_����������� = new List<EGRULSvedDoljnFL> { new EGRULSvedDoljnFL { Id = Guid.NewGuid().ToString(), ���������� = documentXML.����.�����������.�����������?.����������, ��� = documentXML.����.�����������.�����������?.���, �������� = documentXML.����.�����������.����?.��������, ��� = documentXML.����.�����������.����?.���, ������� = documentXML.����.�����������.����?.�������, ��������� = documentXML.����.�����������.�������?.���������, ������������ = documentXML.����.�����������.�������?.������������, ����� = documentXML.����.�����������.����?.�����, �������� = documentXML.����.�����������.�������?.��������, id���� = ULDB.Id } };
                            //}

                            break;
                    }
                }

                //_dbcontext.������.Update(ULDB);
                //try
                //{
                //    _dbcontext.SaveChanges();

                //}
                //catch (DbUpdateConcurrencyException ex)
                //{
                //    // ���� ��������� ������ ������������ ��������� ������ �������� ��� ���
                //    if (documentXML.retry == false)
                //    {
                //        documentXML.retry = true;
                //        Console.WriteLine($"�������� ������ ������������ ��������� ���� {documentXML.����.���}, ����������� ������ ������� ��������");
                //        if (entitiesInWork.Contains(documentXML.����.���))
                //        {
                //            entitiesInWork.Remove(documentXML.����.���);
                //        }
                //        ParseULDataDB((object)documentXML);
                //    }
                //}

                //try
                //{
                //    // ����� ��������� ������ ������� �� �� ������ �������������� 
                //    if (entitiesInWork.Contains(documentXML.����.���))
                //    {
                //        entitiesInWork.Remove(documentXML.����.���);
                //    }
                //}
                //catch (Exception e)
                //{

                //}

                if (docCount - 1 == finishedWorkersCount)
                {
                    _dbcontext.�����_�����.Where(e => ULSubTables.�����_�����_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_�����_Insert.DistinctBy(e => new { e.id����, e.�������� }).ToList());

                    _dbcontext.�����_���������.Where(e => ULSubTables.�����_���������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_���������_Insert);

                    _dbcontext.�����_��������������.Where(e => ULSubTables.�����_��������������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_��������������_Insert);

                    _dbcontext.�����_���������.Where(e => ULSubTables.�����_���������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_���������_Insert.Select(e => e.Entry));

                    _dbcontext.�����_����������.Where(e => ULSubTables.�����_����������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_����������_Insert);

                    _dbcontext.�����_��������.Where(e => ULSubTables.�����_��������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_��������_Insert);

                    _dbcontext.�����_�������.Where(e => ULSubTables.�����_�������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_�������_Insert);

                    _dbcontext.�����_���������.Where(e => ULSubTables.�����_���������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_���������_Insert);

                    _dbcontext.�����_�������.Where(e => ULSubTables.�����_�������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_�������_Insert);

                    _dbcontext.�����_����������.Where(e => ULSubTables.�����_����������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_����������_Insert);

                    _dbcontext.�����_�������.Where(e => ULSubTables.�����_�������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_�������_Insert);

                    _dbcontext.�����_���������.Where(e => ULSubTables.�����_���������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_���������_Insert);

                    _dbcontext.�����_��������.Where(e => ULSubTables.�����_��������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_��������_Insert);

                    _dbcontext.�����_�������.Where(e => ULSubTables.�����_�������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_�������_Insert);

                    _dbcontext.�����_��������.Where(e => ULSubTables.�����_��������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_��������_Insert);

                    _dbcontext.�����_�������.Where(e => ULSubTables.�����_�������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_�������_Insert);

                    _dbcontext.�����_��������.Where(e => ULSubTables.�����_��������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_��������_Insert);

                    _dbcontext.�����_��������.Where(e => ULSubTables.�����_��������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_��������_Insert);

                    _dbcontext.�����_��������.Where(e => ULSubTables.�����_��������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_��������_Insert);

                    _dbcontext.�����_���������.Where(e => ULSubTables.�����_���������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_���������_Insert);

                    _dbcontext.�����_����.Where(e => ULSubTables.�����_����_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_����_Insert);

                    _dbcontext.�����_�����������.Where(e => ULSubTables.�����_�����������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(ULSubTables.�����_�����������_Insert);

                    _dbcontext.BulkCopy(IPSubTables.����������������_Insert);

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
            �������� documentXML = (��������)documentObject;
            bool firstTime = false;

            if (documentXML.���� == null)
            {
                return;
            }

            //while (entitiesInWork.Contains(documentXML.����.�����))
            //{
            //    Console.WriteLine($"���� {documentXML.����.�����} ��� � ���������, �������� ����������.");
            //    Thread.Sleep(5000);
            //}

            //entitiesInWork.Add(documentXML.����.�����);

            using (DbContextTable _dbcontext = _contextFactory.CreateDbContext())
            {

                IP IPDB = new IP();

                IP data = (from u in _dbcontext.�� where u.��� == documentXML.����.����� select u).FirstOrDefault();
                if (data != null)
                {
                    IPDB = data;
                }
                else
                {
                    lock (locker)
                    {
                        data = IPSubTables.IPInProcessing.Where(e => e.��� == documentXML.����.�����).FirstOrDefault();
                    }

                    if (data != null)
                    {
                        IPDB = data;
                    }
                    else
                    {
                        firstTime = true;
                        IPDB = new IP { ���������� = documentXML.����.����������, ��� = documentXML.����.�����, ��������� = documentXML.����.���������, �������� = documentXML.����.��������, ������ = documentXML.����.������, ����� = documentXML.����� };
                        _dbcontext.��.Add(IPDB);
                        _dbcontext.SaveChanges();

                        lock (locker)
                        {
                            IPSubTables.IPInProcessing.Add(IPDB);
                        }
                    }
                }

                Document documentDB = new Document();
                documentDB.������������ = DateTime.Now;
                documentDB.����� = documentXML.�����;
                documentDB.id�� = IPDB.Id;

                IPDB.document = new List<Document> { documentDB };
                _dbcontext.SaveChanges();

                _dbcontext.��.Update(IPDB);

                foreach (var entity in _dbcontext.Model.GetEntityTypes())
                {
                    switch (entity.ShortName())
                    {
                        case "EGRIPSVFL":
                            if (documentXML.����.���� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSVFL previousEntry;
                                var newEntry = new EGRIPSVFL { Id = Guid.NewGuid().ToString(), ��� = documentXML.����.����?.���, �������� = documentXML.����.����?.������?.��������, ��� = documentXML.����.����?.������?.���, ������� = documentXML.����.����?.������?.�������, ���������� = documentXML.����.����?.���������.����������, ����� = documentXML.����.����?.���������.�����, id���� = IPDB.Id };

                                lock (listLockers[0])
                                {
                                    previousEntry = IPSubTables.�����_����_Insert.Where(e => e.id���� == IPDB.Id).FirstOrDefault();
                                }

                                if (previousEntry != null)
                                {
                                    lock (listLockers[0])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.����.���������.����������)
                                        {
                                            IPSubTables.�����_����_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            IPSubTables.�����_����_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.�����_����).Load();

                                        if (IPDB.�����_����?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.�����_����?.FirstOrDefault();
                                            IPSubTables.�����_����_Delete.Add(IPDB.�����_����?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[0])
                                    {
                                        IPSubTables.�����_����_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.����.���������.����������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSVFL).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                ������� = "EGRIPSVFL",
                                                ������� = property.Name,
                                                ���������� = property.GetValue(previousEntry)?.ToString(),
                                                ������������� = property.GetValue(newEntry)?.ToString(),
                                                ��� = documentXML.����.�����,
                                                ������������� = DateTime.Now
                                            };

                                            // //_dbcontext.����������������.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                                //else
                                //{
                                //    lock (listLockers[0])
                                //    {
                                //        IPSubTables.�����_����_Insert.Add(new EGRIPSVFL { Id = IPDB.�����_����?.FirstOrDefault()?.Id != null ? IPDB.�����_����?.FirstOrDefault().Id : Guid.NewGuid().ToString(), ��� = documentXML.����.����?.���, �������� = documentXML.����.����?.������?.��������, ��� = documentXML.����.����?.������?.���, ������� = documentXML.����.����?.������?.�������, ���������� = documentXML.����.����?.���������.����������, ����� = documentXML.����.����?.���������.�����, id���� = IPDB.Id });
                                //    }
                                //}
                            }

                            break;

                        case "EGRIPOKVED":
                            if (documentXML.����.������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                List<EGRIPOKVED> previousEntries;
                                var currentEntries = new List<EGRIPOKVED>();
                                var currentEntry = new EGRIPOKVED { Id = Guid.NewGuid().ToString(), ������ = documentXML.����.�������?.����������?.�����������, ������������ = documentXML.����.�������?.����������?.���������, �������� = documentXML.����.�������?.����������?.��������, ������ = true, ��������� = documentXML.����.�������?.����������?.���������.����������, ����� = documentXML.����.�������?.����������?.���������.�����, id���� = IPDB.Id };
                                DateTime? currentEntriesDate = documentXML.����.�������.���������� != null ? documentXML.����.�������.����������?.���������.���������� : documentXML.����.�������.����������?.FirstOrDefault().���������.����������;

                                lock (listLockers[1])
                                {
                                    previousEntries = IPSubTables.�����_�����_Insert.Where(e => e.id���� == IPDB.Id).ToList();
                                }
                                if (previousEntries.Count != 0)
                                {
                                    lock (listLockers[1])
                                    {
                                        if (previousEntries.FirstOrDefault()?.��������� <= currentEntriesDate)
                                        {
                                            IPSubTables.�����_�����_Insert.RemoveAll(e => e.id���� == IPDB.Id);
                                            IPSubTables.�����_�����_Insert.Add(currentEntry);

                                            foreach (���������� okvedDop in documentXML.����.�������?.����������)
                                            {
                                                currentEntry = new EGRIPOKVED { Id = Guid.NewGuid().ToString(), �������� = okvedDop.��������, ������ = false, ������������ = okvedDop.���������, ������ = okvedDop.�����������, ����� = okvedDop.���������.�����, ��������� = okvedDop.���������.����������, id���� = IPDB.Id };
                                                currentEntries.Add(currentEntry);
                                                lock (listLockers[1])
                                                {
                                                    IPSubTables.�����_�����_Insert.Add(currentEntry);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.�����_�����).Load();

                                        if (IPDB.�����_�����?.FirstOrDefault() != null)
                                        {
                                            previousEntries = IPDB.�����_�����.ToList();
                                            IPSubTables.�����_�����_Delete.Add(IPDB.�����_�����?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[1])
                                    {
                                        IPSubTables.�����_�����_Insert.Add(currentEntry);
                                    }

                                    foreach (���������� okvedDop in documentXML.����.�������?.����������)
                                    {
                                        currentEntry = new EGRIPOKVED { Id = Guid.NewGuid().ToString(), �������� = okvedDop.��������, ������ = false, ������������ = okvedDop.���������, ������ = okvedDop.�����������, ����� = okvedDop.���������.�����, ��������� = okvedDop.���������.����������, id���� = IPDB.Id };
                                        currentEntries.Add(currentEntry);
                                        lock (listLockers[1])
                                        {
                                            IPSubTables.�����_�����_Insert.Add(currentEntry);
                                        }
                                    }
                                }

                                if (previousEntries?.Count > 0 && previousEntries.FirstOrDefault()?.��������� <= currentEntriesDate)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var oldEntry in previousEntries)
                                    {
                                        var newEntry = currentEntries.Where(e => e.�������� == oldEntry.�������� || e.������������ == oldEntry.������������).FirstOrDefault();

                                        if (newEntry == null)
                                        {
                                            continue;
                                        }

                                        foreach (var property in typeof(EGRIPOKVED).GetProperties())
                                        {
                                            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                            {
                                                if (property.Name == "Id" || property.Name == "id����" || property.Name == "��")
                                                    continue;

                                                ChangeLog changes = new ChangeLog();

                                                changes.������� = "EGRIPOKVED";
                                                changes.������� = property.Name;
                                                changes.���������� = property.GetValue(oldEntry)?.ToString();
                                                changes.������������� = property.GetValue(newEntry)?.ToString();
                                                changes.��� = documentXML.����.�����;
                                                changes.������������� = DateTime.Now;

                                                //_dbcontext.����������������.Add(changes);
                                                changeList.Add(changes);
                                            }
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                                //else
                                //{
                                //    lock (listLockers[1])
                                //    {
                                //        IPSubTables.�����_�����_Insert.Add(new EGRIPOKVED { Id = Guid.NewGuid().ToString(), ������ = documentXML.����.�������?.����������?.�����������, ������������ = documentXML.����.�������?.����������?.���������, �������� = documentXML.����.�������?.����������?.��������, ������ = true, ��������� = documentXML.����.�������?.����������?.���������.����������, ����� = documentXML.����.�������?.����������?.���������.�����, id���� = IPDB.Id });

                                //        foreach (���������� okvedDop in documentXML.����.�������?.����������)
                                //        {
                                //            IPSubTables.�����_�����_Insert.Add(new EGRIPOKVED { Id = Guid.NewGuid().ToString(), �������� = okvedDop.��������, ������ = false, ������������ = okvedDop.���������, ������ = okvedDop.�����������, ����� = okvedDop.���������.�����, ��������� = okvedDop.���������.����������, id���� = IPDB.Id });
                                //        }
                                //    }                                    
                                //}
                            }

                            break;

                        case "EGRIPSvAdrMJ":

                            if (documentXML.����.������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvAdrMJ previousEntry;
                                var newEntry = new EGRIPSvAdrMJ { Id = Guid.NewGuid().ToString(), ���������� = documentXML.����.�������.���������.����������, ����� = documentXML.����.�������.���������.�����, ��������� = documentXML.����.�������.�������?.���������, ��������� = documentXML.����.�������.�������?.�����?.���������, �������������� = documentXML.����.�������.�������?.����������?.��������������, ��������� = documentXML.����.�������.�������?.�����?.���������, ���������� = documentXML.����.�������.�������?.������?.����������, �������� = documentXML.����.�������.�������?.�����?.��������, ������������� = documentXML.����.�������.�������?.����������?.�������������, �������� = documentXML.����.�������.�������?.�����?.��������, ��������� = documentXML.����.�������.�������?.������?.���������, id���� = IPDB.Id };

                                lock (listLockers[2])
                                {
                                    previousEntry = IPSubTables.�����_�������_Insert.Where(e => e.id���� == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[2])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.�������.���������.����������)
                                        {
                                            IPSubTables.�����_�������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            IPSubTables.�����_�������_Insert.Add(newEntry);
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.�����_�������).Load();

                                        if (IPDB.�����_�������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.�����_�������?.FirstOrDefault();
                                            IPSubTables.�����_�������_Delete.Add(IPDB.�����_�������?.FirstOrDefault().id����.ToString());
                                        }

                                    }

                                    lock (listLockers[2])
                                    {
                                        IPSubTables.�����_�������_Insert.Add(newEntry);
                                    }
                                }



                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.�������.���������.����������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvAdrMJ).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                ������� = "EGRIPSvAdrMJ",
                                                ������� = property.Name,
                                                ���������� = property.GetValue(previousEntry)?.ToString(),
                                                ������������� = property.GetValue(newEntry)?.ToString(),
                                                ��� = documentXML.����.�����,
                                                ������������� = DateTime.Now
                                            };

                                            // //_dbcontext.����������������.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }

                                //else
                                //{                               
                                //_dbcontext.Entry(IPDB).Collection(u => u.�����_�������).Load();

                                //if (IPDB.�����_�������.FirstOrDefault() != null)
                                //{
                                //    _dbcontext.Entry(IPDB.�����_�������.FirstOrDefault()).State = EntityState.Deleted;
                                //    var previousEntry = IPDB.�����_�������.FirstOrDefault();
                                //    IPDB.�����_������� = new List<EGRIPSvAdrMJ> {  };
                                //    _dbcontext.Entry(IPDB.�����_�������.FirstOrDefault()).State = EntityState.Added;

                                //    List<ChangeLog> changeList = new List<ChangeLog>();

                                //    foreach (var property in typeof(EGRIPSvAdrMJ).GetProperties())
                                //    {
                                //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.�����_�������.FirstOrDefault())?.ToString())
                                //        {
                                //            ChangeLog changes = new ChangeLog();

                                //            changes.������� = "EGRIPSvAdrMJ";
                                //            changes.������� = property.Name;
                                //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                //            changes.������������� = property.GetValue(IPDB.�����_�������.FirstOrDefault())?.ToString();
                                //            changes.��� = documentXML.����.�����;
                                //            changes.������������� = DateTime.Now;

                                //            //_dbcontext.����������������.Add(changes);
                                //            changeList.Add(changes);
                                //        }
                                //    }

                                //    _dbcontext.����������������.AddRange(changeList);
                                //}
                                //else
                                //{
                                //    IPDB.�����_������� = new List<EGRIPSvAdrMJ> { new EGRIPSvAdrMJ { Id = Guid.NewGuid().ToString(), ���������� = documentXML.����.�������.���������.����������, ����� = documentXML.����.�������.���������.�����, ��������� = documentXML.����.�������.�������?.���������, ��������� = documentXML.����.�������.�������?.�����?.���������, �������������� = documentXML.����.�������.�������?.����������?.��������������, ��������� = documentXML.����.�������.�������?.�����?.���������, ���������� = documentXML.����.�������.�������?.������?.����������, �������� = documentXML.����.�������.�������?.�����?.��������, ������������� = documentXML.����.�������.�������?.����������?.�������������, �������� = documentXML.����.�������.�������?.�����?.��������, ��������� = documentXML.����.�������.�������?.������?.���������, id���� = IPDB.Id } };
                                //}
                            }

                            break;

                        case "EGRIPSvGrajd":
                            if (documentXML.����.������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvGrajd previousEntry;
                                var newEntry = new EGRIPSvGrajd { Id = Guid.NewGuid().ToString(), �������� = documentXML.����.�������.��������, ���������� = documentXML.����.�������.���������.����������, ����� = documentXML.����.�������.���������.�����, ��������� = documentXML.����.�������.���������, ���� = documentXML.����.�������.����, id���� = IPDB.Id };

                                lock (listLockers[3])
                                {
                                    previousEntry = IPSubTables.�����_�������_Insert.Where(e => e.id���� == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[3])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.�������.���������.����������)
                                        {
                                            IPSubTables.�����_�������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            IPSubTables.�����_�������_Insert.Add(newEntry);
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.�����_�������).Load();

                                        if (IPDB.�����_�������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.�����_�������?.FirstOrDefault();
                                            IPSubTables.�����_�������_Delete.Add(IPDB.�����_�������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[3])
                                    {
                                        IPSubTables.�����_�������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.�������.���������.����������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvGrajd).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                ������� = "EGRIPSvGrajd",
                                                ������� = property.Name,
                                                ���������� = property.GetValue(previousEntry)?.ToString(),
                                                ������������� = property.GetValue(newEntry)?.ToString(),
                                                ��� = documentXML.����.�����,
                                                ������������� = DateTime.Now
                                            };

                                            // //_dbcontext.����������������.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }


                                //_dbcontext.Entry(IPDB).Collection(u => u.�����_�������).Load();

                                //if (IPDB.�����_�������.FirstOrDefault() != null)
                                //{
                                //    _dbcontext.Entry(IPDB.�����_�������.FirstOrDefault()).State = EntityState.Deleted;
                                //    var previousEntry = IPDB.�����_�������.FirstOrDefault();
                                //    IPDB.�����_������� = new List<EGRIPSvGrajd> { new EGRIPSvGrajd { Id = IPDB.�����_�������.FirstOrDefault()?.Id != null ? IPDB.�����_�������.FirstOrDefault().Id : Guid.NewGuid().ToString(), �������� = documentXML.����.�������.��������, ���������� = documentXML.����.�������.���������.����������, ����� = documentXML.����.�������.���������.�����, ��������� = documentXML.����.�������.���������, ���� = documentXML.����.�������.����, id���� = IPDB.Id } };
                                //    _dbcontext.Entry(IPDB.�����_�������.FirstOrDefault()).State = EntityState.Added;

                                //    List<ChangeLog> changeList = new List<ChangeLog>();

                                //    foreach (var property in typeof(EGRIPSvGrajd).GetProperties())
                                //    {
                                //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.�����_�������.FirstOrDefault())?.ToString())
                                //        {
                                //            ChangeLog changes = new ChangeLog();

                                //            changes.������� = "EGRIPSvGrajd";
                                //            changes.������� = property.Name;
                                //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                                //            changes.������������� = property.GetValue(IPDB.�����_�������.FirstOrDefault())?.ToString();
                                //            changes.��� = documentXML.����.�����;
                                //            changes.������������� = DateTime.Now;

                                //            //_dbcontext.����������������.Add(changes);
                                //            changeList.Add(changes);
                                //        }
                                //    }

                                //    _dbcontext.����������������.AddRange(changeList);
                                //}
                                //else
                                //{
                                //    IPDB.�����_������� = new List<EGRIPSvGrajd> { new EGRIPSvGrajd { Id = Guid.NewGuid().ToString(), �������� = documentXML.����.�������.��������, ���������� = documentXML.����.�������.���������.����������, ����� = documentXML.����.�������.���������.�����, ��������� = documentXML.����.�������.���������, ���� = documentXML.����.�������.����, id���� = IPDB.Id } };
                                //}
                            }

                            break;

                        case "EGRIPSvIP":
                            if (documentXML.���� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvIP previousEntry;
                                var newEntry = new EGRIPSvIP { Id = Guid.NewGuid().ToString(), ��������� = documentXML.����.���������, �������� = documentXML.����.��������, ��� = documentXML.����.�����, �������� = documentXML.����.����������, ���� = documentXML.����.������, ������� = documentXML.����.�������, �������� = documentXML.����.����?.������?.������� + " " + documentXML.����.����?.������?.��� + " " + documentXML.����.����?.������?.��������, �������� = documentXML.����.�������?.����������?.��������, id���� = IPDB.Id };

                                lock (listLockers[4])
                                {
                                    previousEntry = IPSubTables.�����_����_Insert.Where(e => e.id���� == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[4])
                                    {
                                        if (previousEntry.�������� <= documentXML.����.����������)
                                        {
                                            IPSubTables.�����_����_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            IPSubTables.�����_����_Insert.Add(newEntry);
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.�����_����).Load();

                                        if (IPDB.�����_����?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.�����_����?.FirstOrDefault();
                                            IPSubTables.�����_����_Delete.Add(IPDB.�����_����?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[4])
                                    {
                                        IPSubTables.�����_����_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.�������� <= documentXML.����.����������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvIP).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                ������� = "EGRIPSvIP",
                                                ������� = property.Name,
                                                ���������� = property.GetValue(previousEntry)?.ToString(),
                                                ������������� = property.GetValue(newEntry)?.ToString(),
                                                ��� = documentXML.����.�����,
                                                ������������� = DateTime.Now
                                            };

                                            // //_dbcontext.����������������.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.�����_����).Load();

                            //if (IPDB.�����_����.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.�����_����.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.�����_����.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.�����_���� = new List<EGRIPSvIP> { new EGRIPSvIP { Id = IPDB.�����_����.FirstOrDefault()?.Id != null ? IPDB.�����_����.FirstOrDefault().Id : Guid.NewGuid().ToString(), ��������� = documentXML.����.���������, �������� = documentXML.����.��������, ��� = documentXML.����.�����, �������� = documentXML.����.����������, ���� = documentXML.����.������, ������� = documentXML.����.�������, �������� = documentXML.����.����?.������?.������� + " " + documentXML.����.����?.������?.��� + " " + documentXML.����.����?.������?.��������, �������� = documentXML.����.�������?.����������?.��������, id���� = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.�����_����.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvIP).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.�����_����.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRIPSvIP";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(IPDB.�����_����.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.�����;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.����������������.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.�����_���� = new List<EGRIPSvIP> { new EGRIPSvIP { Id = Guid.NewGuid().ToString(), ��������� = documentXML.����.���������, �������� = documentXML.����.��������, ��� = documentXML.����.�����, �������� = documentXML.����.����������, ���� = documentXML.����.������, ������� = documentXML.����.�������, �������� = documentXML.����.����?.������?.������� + " " + documentXML.����.����?.������?.��� + " " + documentXML.����.����?.������?.��������, �������� = documentXML.����.�������?.����������?.��������, id���� = IPDB.Id } };
                            //}

                            break;

                        case "EGRIPSvLicense":
                            if (documentXML.����.���������� == null)
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
                                    foreach (var entryDatePair in IPSubTables.�����_����������_Insert.Where(e => e.Entry.id���� == IPDB.Id))
                                    {
                                        previousEntries.Add(entryDatePair.Entry);
                                    }

                                    var licenseArray = IPSubTables.�����_����������_Insert.Where(e => e.Entry.id���� == IPDB.Id).Select(e => e.Date).ToArray();
                                    previousEntriesDate = licenseArray.Count() > 0 ? licenseArray.Max() : DateTime.MinValue;
                                }
                                if (previousEntries.Count != 0)
                                {
                                    lock (listLockers[5])
                                    {
                                        if (previousEntriesDate <= documentXML.����.�������)
                                        {
                                            IPSubTables.�����_����������_Insert.RemoveAll(e => e.Entry.id���� == IPDB.Id);

                                            foreach (var svLicense in documentXML.����.����������)
                                            {
                                                currentEntry = new EGRIPSvLicense { Id = Guid.NewGuid().ToString(), ���������� = svLicense.����������, ������� = svLicense.�������, ������ = svLicense.������, �������������� = svLicense.��������������, ������������ = svLicense.������������, ������������ = svLicense.������������, ���������� = svLicense.���������.����������, ����� = svLicense.���������.�����, id���� = IPDB.Id };
                                                currentEntries.Add(currentEntry);
                                                IPSubTables.�����_����������_Insert.Add((currentEntry, documentXML.����.�������));
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.�����_����������).Load();
                                        if (IPDB.�����_����������?.FirstOrDefault() != null)
                                        {
                                            previousEntries = IPDB.�����_����������.ToList();
                                            IPSubTables.�����_����������_Delete.Add(IPDB.�����_����������?.FirstOrDefault().id����.ToString());
                                        }
                                    }
                                    lock (listLockers[5])
                                    {
                                        foreach (var svLicense in documentXML.����.����������)
                                        {
                                            currentEntry = new EGRIPSvLicense { Id = Guid.NewGuid().ToString(), ���������� = svLicense.����������, ������� = svLicense.�������, ������ = svLicense.������, �������������� = svLicense.��������������, ������������ = svLicense.������������, ������������ = svLicense.������������, ���������� = svLicense.���������.����������, ����� = svLicense.���������.�����, id���� = IPDB.Id };
                                            currentEntries.Add(currentEntry);
                                            IPSubTables.�����_����������_Insert.Add((currentEntry, documentXML.����.�������));
                                        }
                                    }
                                }
                                if (previousEntries?.Count > 0 && previousEntriesDate <= documentXML.����.�������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();
                                    foreach (var oldEntry in previousEntries)
                                    {
                                        var newEntry = currentEntries.Where(e => e.������ == oldEntry.������).FirstOrDefault();
                                        if (newEntry == null)
                                        {
                                            continue;
                                        }
                                        foreach (var property in typeof(EGRIPSvLicense).GetProperties())
                                        {

                                            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                            {
                                                if (property.Name == "Id" || property.Name == "id����" || property.Name == "��")
                                                    continue;

                                                ChangeLog changes = new ChangeLog();

                                                changes.������� = "EGRIPSvLicense";
                                                changes.������� = property.Name;
                                                changes.���������� = property.GetValue(oldEntry)?.ToString();
                                                changes.������������� = property.GetValue(newEntry)?.ToString();
                                                changes.��� = documentXML.����.�����;
                                                changes.������������� = DateTime.Now;

                                                //_dbcontext.����������������.Add(changes);
                                                changeList.Add(changes);
                                            }
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }

                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.�����_����������).Load();

                            //if (IPDB.�����_����������.Count > 0)
                            //{
                            //    foreach (EGRIPSvLicense entry in IPDB.�����_����������)
                            //    {
                            //        _dbcontext.Entry(entry).State = EntityState.Deleted;
                            //    }

                            //    var previousEntries = IPDB.�����_����������;
                            //    IPDB.�����_���������� = new List<EGRIPSvLicense>();

                            //    foreach (var svLicense in documentXML.����.����������)
                            //    {
                            //        IPDB.�����_����������.Add(new EGRIPSvLicense { Id = Guid.NewGuid().ToString(), ���������� = svLicense.����������, ������� = svLicense.�������, ������ = svLicense.������, �������������� = svLicense.��������������, ������������ = svLicense.������������, ������������ = svLicense.������������, ���������� = svLicense.���������.����������, ����� = svLicense.���������.�����, id���� = IPDB.Id });
                            //        _dbcontext.Entry(IPDB.�����_����������.Last()).State = EntityState.Added;
                            //    }

                            //    var currentEntries = IPDB.�����_����������;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var oldEntry in previousEntries)
                            //    {
                            //        var newEntry = currentEntries.Where(e => e.������ == oldEntry.������).FirstOrDefault();

                            //        if (newEntry == null)
                            //        {
                            //            continue;
                            //        }

                            //        foreach (var property in typeof(EGRIPSvLicense).GetProperties())
                            //        {

                            //            if (property.GetValue(oldEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                            //            {
                            //                if (property.Name == "Id" || property.Name == "id����" || property.Name == "��")
                            //                    continue;

                            //                ChangeLog changes = new ChangeLog();

                            //                changes.������� = "EGRIPSvLicense";
                            //                changes.������� = property.Name;
                            //                changes.���������� = property.GetValue(oldEntry)?.ToString();
                            //                changes.������������� = property.GetValue(newEntry)?.ToString();
                            //                changes.��� = documentXML.����.�����;
                            //                changes.������������� = DateTime.Now;

                            //                //_dbcontext.����������������.Add(changes);
                            //                changeList.Add(changes);
                            //            }
                            //        }
                            //    }

                            //    _dbcontext.����������������.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.�����_���������� = new List<EGRIPSvLicense>();

                            //    foreach (var svLicense in documentXML.����.����������)
                            //    {
                            //        IPDB.�����_����������.Add(new EGRIPSvLicense { Id = Guid.NewGuid().ToString(), ���������� = svLicense.����������, ������� = svLicense.�������, ������ = svLicense.������, �������������� = svLicense.��������������, ������������ = svLicense.������������, ������������ = svLicense.������������, ���������� = svLicense.���������.����������, ����� = svLicense.���������.�����, id���� = IPDB.Id });
                            //    }
                            //}

                            break;

                        case "EGRIPSvPrekras_":
                            if (documentXML.����.��������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvPrekras_ previousEntry;
                                var newEntry = new EGRIPSvPrekras_ { Id = Guid.NewGuid().ToString(), ����������� = documentXML.����.���������.��������?.�����������, ��������� = documentXML.����.���������.��������?.���������, ���������� = documentXML.����.���������.��������?.����������, ���������� = documentXML.����.���������.���������.����������, ����� = documentXML.����.���������.���������.�����, id���� = IPDB.Id };

                                lock (listLockers[6])
                                {
                                    previousEntry = IPSubTables.�����_���������_Insert.Where(e => e.id���� == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[6])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.���������.���������.����������)
                                        {
                                            IPSubTables.�����_���������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            IPSubTables.�����_���������_Insert.Add(newEntry);
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.�����_���������).Load();

                                        if (IPDB.�����_���������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.�����_���������?.FirstOrDefault();
                                            IPSubTables.�����_���������_Delete.Add(IPDB.�����_���������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[6])
                                    {
                                        IPSubTables.�����_���������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.���������.���������.����������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvPrekras_).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                ������� = "EGRIPSvPrekras_",
                                                ������� = property.Name,
                                                ���������� = property.GetValue(previousEntry)?.ToString(),
                                                ������������� = property.GetValue(newEntry)?.ToString(),
                                                ��� = documentXML.����.�����,
                                                ������������� = DateTime.Now
                                            };

                                            // //_dbcontext.����������������.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.�����_���������).Load();

                            //if (IPDB.�����_���������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.�����_���������.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.�����_���������.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.�����_��������� = new List<EGRIPSvPrekras_> { new EGRIPSvPrekras_ { Id = IPDB.�����_���������.FirstOrDefault()?.Id != null ? IPDB.�����_���������.FirstOrDefault().Id : Guid.NewGuid().ToString(), ����������� = documentXML.����.���������.��������?.�����������, ��������� = documentXML.����.���������.��������?.���������, ���������� = documentXML.����.���������.��������?.����������, ���������� = documentXML.����.���������.���������.����������, ����� = documentXML.����.���������.���������.�����, id���� = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.�����_���������.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvPrekras_).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.�����_���������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRIPSvPrekras_";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(IPDB.�����_���������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.�����;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.����������������.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.�����_��������� = new List<EGRIPSvPrekras_> { new EGRIPSvPrekras_ { Id = Guid.NewGuid().ToString(), ����������� = documentXML.����.���������.��������?.�����������, ��������� = documentXML.����.���������.��������?.���������, ���������� = documentXML.����.���������.��������?.����������, ���������� = documentXML.����.���������.���������.����������, ����� = documentXML.����.���������.���������.�����, id���� = IPDB.Id } };
                            //}

                            break;

                        case "EGRIPSvRegIP":
                            if (documentXML.����.������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvRegIP previousEntry;
                                var newEntry = new EGRIPSvRegIP { Id = Guid.NewGuid().ToString(), ���������� = documentXML.����.�������.����������, ����� = documentXML.����.�������.�����, ���������� = documentXML.����.�������.����������, ������� = documentXML.����.�������.�������, ��� = documentXML.����.�������.���, ������ = documentXML.����.�������.������, ���������� = documentXML.����.�������.����������, ���� = documentXML.����.�������.����, ������ = documentXML.����.�������.������, ������ = documentXML.����.�������.������, id���� = IPDB.Id };

                                lock (listLockers[7])
                                {
                                    previousEntry = IPSubTables.�����_�������_Insert.Where(e => e.id���� == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[7])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.�������.����������)
                                        {
                                            IPSubTables.�����_�������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            IPSubTables.�����_�������_Insert.Add(newEntry);
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.�����_�������).Load();

                                        if (IPDB.�����_�������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.�����_�������?.FirstOrDefault();
                                            IPSubTables.�����_�������_Delete.Add(IPDB.�����_�������?.FirstOrDefault().id����.ToString());
                                        }
                                    }
                                    lock (listLockers[7])
                                    {
                                        IPSubTables.�����_�������_Insert.Add(newEntry);
                                    }
                                }



                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.�������.����������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvRegIP).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                ������� = "EGRIPSvRegIP",
                                                ������� = property.Name,
                                                ���������� = property.GetValue(previousEntry)?.ToString(),
                                                ������������� = property.GetValue(newEntry)?.ToString(),
                                                ��� = documentXML.����.�����,
                                                ������������� = DateTime.Now
                                            };

                                            // //_dbcontext.����������������.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.�����_�������).Load();

                            //if (IPDB.�����_�������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.�����_�������.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.�����_�������.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.�����_������� = new List<EGRIPSvRegIP> { new EGRIPSvRegIP { Id = IPDB.�����_�������.FirstOrDefault()?.Id != null ? IPDB.�����_�������.FirstOrDefault().Id : Guid.NewGuid().ToString(), ���������� = documentXML.����.�������.����������, ����� = documentXML.����.�������.�����, ���������� = documentXML.����.�������.����������, ������� = documentXML.����.�������.�������, ��� = documentXML.����.�������.���, ������ = documentXML.����.�������.������, ���������� = documentXML.����.�������.����������, ���� = documentXML.����.�������.����, ������ = documentXML.����.�������.������, ������ = documentXML.����.�������.������, id���� = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.�����_�������.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvRegIP).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.�����_�������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRIPSvRegIP";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(IPDB.�����_�������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.�����;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.����������������.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.�����_������� = new List<EGRIPSvRegIP> { new EGRIPSvRegIP { Id = Guid.NewGuid().ToString(), ���������� = documentXML.����.�������.����������, ����� = documentXML.����.�������.�����, ���������� = documentXML.����.�������.����������, ������� = documentXML.����.�������.�������, ��� = documentXML.����.�������.���, ������ = documentXML.����.�������.������, ���������� = documentXML.����.�������.����������, ���� = documentXML.����.�������.����, ������ = documentXML.����.�������.������, ������ = documentXML.����.�������.������, id���� = IPDB.Id } };
                            //}

                            break;

                        case "EGRIPSvGegOrg":
                            if (documentXML.����.�������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvGegOrg previousEntry;
                                var newEntry = new EGRIPSvGegOrg { Id = Guid.NewGuid().ToString(), ����� = documentXML.����.��������?.�����, ����� = documentXML.����.��������?.�����, ������ = documentXML.����.��������?.������, ���������� = documentXML.����.��������?.���������.����������, ����� = documentXML.����.��������?.���������.�����, id���� = IPDB.Id };

                                lock (listLockers[8])
                                {
                                    previousEntry = IPSubTables.�����_��������_Insert.Where(e => e.id���� == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[8])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.��������.���������.����������)
                                        {
                                            IPSubTables.�����_��������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            IPSubTables.�����_��������_Insert.Add(newEntry);
                                        }
                                    }
                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.�����_��������).Load();

                                        if (IPDB.�����_��������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.�����_��������?.FirstOrDefault();
                                            IPSubTables.�����_��������_Delete.Add(IPDB.�����_��������?.FirstOrDefault().id����.ToString());
                                        }
                                    }
                                    lock (listLockers[8])
                                    {
                                        IPSubTables.�����_��������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.��������.���������.����������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvGegOrg).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                ������� = "EGRIPSvGegOrg",
                                                ������� = property.Name,
                                                ���������� = property.GetValue(previousEntry)?.ToString(),
                                                ������������� = property.GetValue(newEntry)?.ToString(),
                                                ��� = documentXML.����.�����,
                                                ������������� = DateTime.Now
                                            };

                                            // //_dbcontext.����������������.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.�����_��������).Load();

                            //if (IPDB.�����_��������?.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.�����_��������?.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.�����_��������?.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.�����_�������� = new List<EGRIPSvGegOrg> { new EGRIPSvGegOrg { Id = IPDB.�����_��������?.FirstOrDefault()?.Id != null ? IPDB.�����_��������?.FirstOrDefault().Id : Guid.NewGuid().ToString(), ����� = documentXML.����.��������?.�����, ����� = documentXML.����.��������?.�����, ������ = documentXML.����.��������?.������, ���������� = documentXML.����.��������?.���������.����������, ����� = documentXML.����.��������?.���������.�����, id���� = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.�����_��������?.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvGegOrg).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.�����_��������?.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRIPSvGegOrg";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(IPDB.�����_��������?.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.�����;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.����������������.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.�����_�������� = new List<EGRIPSvGegOrg> { new EGRIPSvGegOrg { Id = Guid.NewGuid().ToString(), ����� = documentXML.����.��������?.�����, ����� = documentXML.����.��������?.�����, ������ = documentXML.����.��������?.������, ���������� = documentXML.����.��������?.���������.����������, ����� = documentXML.����.��������?.���������.�����, id���� = IPDB.Id } };
                            //}

                            break;

                        case "EGRIPSvRegPF":
                            if (documentXML.����.������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvRegPF previousEntry;
                                var newEntry = new EGRIPSvRegPF { Id = Guid.NewGuid().ToString(), ������� = documentXML.����.�������.�������, �������� = documentXML.����.�������.��������, ������ = documentXML.����.�������.�������?.������, ����� = documentXML.����.�������.�������?.�����, ���������� = documentXML.����.�������.���������.����������, ����� = documentXML.����.�������.���������.�����, id���� = IPDB.Id };

                                lock (listLockers[9])
                                {
                                    previousEntry = IPSubTables.�����_�������_Insert.Where(e => e.id���� == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[9])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.�������.���������.����������)
                                        {
                                            IPSubTables.�����_�������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            IPSubTables.�����_�������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.�����_�������).Load();

                                        if (IPDB.�����_�������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.�����_�������?.FirstOrDefault();
                                            IPSubTables.�����_�������_Delete.Add(IPDB.�����_�������?.FirstOrDefault().id����.ToString());
                                        }
                                    }
                                    lock (listLockers[9])
                                    {
                                        IPSubTables.�����_�������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.�������.���������.����������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvRegPF).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                ������� = "EGRIPSvRegPF",
                                                ������� = property.Name,
                                                ���������� = property.GetValue(previousEntry)?.ToString(),
                                                ������������� = property.GetValue(newEntry)?.ToString(),
                                                ��� = documentXML.����.�����,
                                                ������������� = DateTime.Now
                                            };

                                            // //_dbcontext.����������������.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.�����_�������).Load();

                            //if (IPDB.�����_�������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.�����_�������.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.�����_�������.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.�����_������� = new List<EGRIPSvRegPF> { new EGRIPSvRegPF { Id = IPDB.�����_�������.FirstOrDefault()?.Id != null ? IPDB.�����_�������.FirstOrDefault().Id : Guid.NewGuid().ToString(), ������� = documentXML.����.�������.�������, �������� = documentXML.����.�������.��������, ������ = documentXML.����.�������.�������?.������, ����� = documentXML.����.�������.�������?.�����, ���������� = documentXML.����.�������.���������.����������, ����� = documentXML.����.�������.���������.�����, id���� = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.�����_�������.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvRegPF).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.�����_�������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRIPSvRegPF";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(IPDB.�����_�������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.�����;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.����������������.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.�����_������� = new List<EGRIPSvRegPF> { new EGRIPSvRegPF { Id = Guid.NewGuid().ToString(), ������� = documentXML.����.�������.�������, �������� = documentXML.����.�������.��������, ������ = documentXML.����.�������.�������?.������, ����� = documentXML.����.�������.�������?.�����, ���������� = documentXML.����.�������.���������.����������, ����� = documentXML.����.�������.���������.�����, id���� = IPDB.Id } };
                            //}

                            break;

                        case "EGRIPSvRegFSS":
                            if (documentXML.����.�������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvRegFSS previousEntry;
                                var newEntry = new EGRIPSvRegFSS { Id = Guid.NewGuid().ToString(), ������� = documentXML.����.��������.�������, ��������� = documentXML.����.��������.���������, ������� = documentXML.����.��������.��������?.�������, ������ = documentXML.����.��������.��������?.������, ���������� = documentXML.����.��������.���������.����������, ����� = documentXML.����.��������.���������.�����, id���� = IPDB.Id };

                                lock (listLockers[10])
                                {
                                    previousEntry = IPSubTables.�����_��������_Insert.Where(e => e.id���� == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[10])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.��������.���������.����������)
                                        {
                                            IPSubTables.�����_��������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            IPSubTables.�����_��������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.�����_��������).Load();

                                        if (IPDB.�����_��������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.�����_��������?.FirstOrDefault();
                                            IPSubTables.�����_��������_Delete.Add(IPDB.�����_��������?.FirstOrDefault().id����.ToString());
                                        }
                                    }
                                    lock (listLockers[10])
                                    {
                                        IPSubTables.�����_��������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.��������.���������.����������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvRegFSS).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                ������� = "EGRIPSvRegFSS",
                                                ������� = property.Name,
                                                ���������� = property.GetValue(previousEntry)?.ToString(),
                                                ������������� = property.GetValue(newEntry)?.ToString(),
                                                ��� = documentXML.����.�����,
                                                ������������� = DateTime.Now
                                            };

                                            // //_dbcontext.����������������.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.�����_��������).Load();

                            //if (IPDB.�����_��������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.�����_��������.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.�����_��������.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.�����_�������� = new List<EGRIPSvRegFSS> { new EGRIPSvRegFSS { Id = IPDB.�����_��������.FirstOrDefault()?.Id != null ? IPDB.�����_��������.FirstOrDefault().Id : Guid.NewGuid().ToString(), ������� = documentXML.����.��������.�������, ��������� = documentXML.����.��������.���������, ������� = documentXML.����.��������.��������?.�������, ������ = documentXML.����.��������.��������?.������, ���������� = documentXML.����.��������.���������.����������, ����� = documentXML.����.��������.���������.�����, id���� = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.�����_��������.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvRegFSS).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.�����_��������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRIPSvRegFSS";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(IPDB.�����_��������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.�����;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.����������������.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.�����_�������� = new List<EGRIPSvRegFSS> { new EGRIPSvRegFSS { Id = Guid.NewGuid().ToString(), ������� = documentXML.����.��������.�������, ��������� = documentXML.����.��������.���������, ������� = documentXML.����.��������.��������?.�������, ������ = documentXML.����.��������.��������?.������, ���������� = documentXML.����.��������.���������.����������, ����� = documentXML.����.��������.���������.�����, id���� = IPDB.Id } };
                            //}

                            break;

                        case "EGRIPSvAccountingNO":
                            if (documentXML.����.�������� == null)
                            {
                                continue;
                            }
                            else
                            {
                                EGRIPSvAccountingNO previousEntry;
                                var newEntry = new EGRIPSvAccountingNO { Id = Guid.NewGuid().ToString(), ����� = documentXML.����.��������.�����, ���������� = documentXML.����.��������.����������, ������ = documentXML.����.��������.����.������, ����� = documentXML.����.��������.����.������, ���������� = documentXML.����.��������.���������.����������, ����� = documentXML.����.��������.���������.�����, id���� = IPDB.Id };

                                lock (listLockers[11])
                                {
                                    previousEntry = IPSubTables.�����_��������_Insert.Where(e => e.id���� == IPDB.Id).FirstOrDefault();
                                }
                                if (previousEntry != null)
                                {
                                    lock (listLockers[11])
                                    {
                                        if (previousEntry.���������� <= documentXML.����.��������.���������.����������)
                                        {
                                            IPSubTables.�����_��������_Insert.RemoveAll(e => e.id���� == previousEntry.id����);
                                            IPSubTables.�����_��������_Insert.Add(newEntry);
                                        }
                                    }

                                }
                                else
                                {
                                    if (firstTime == false)
                                    {
                                        _dbcontext.Entry(IPDB).Collection(u => u.�����_��������).Load();

                                        if (IPDB.�����_��������?.FirstOrDefault() != null)
                                        {
                                            previousEntry = IPDB.�����_��������?.FirstOrDefault();
                                            IPSubTables.�����_��������_Delete.Add(IPDB.�����_��������?.FirstOrDefault().id����.ToString());
                                        }
                                    }

                                    lock (listLockers[11])
                                    {
                                        IPSubTables.�����_��������_Insert.Add(newEntry);
                                    }
                                }

                                if (previousEntry != null && previousEntry.���������� <= documentXML.����.��������.���������.����������)
                                {
                                    List<ChangeLog> changeList = new List<ChangeLog>();

                                    foreach (var property in typeof(EGRIPSvAccountingNO).GetProperties())
                                    {
                                        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(newEntry)?.ToString())
                                        {
                                            ChangeLog changes = new()
                                            {
                                                ������� = "EGRIPSvAccountingNO",
                                                ������� = property.Name,
                                                ���������� = property.GetValue(previousEntry)?.ToString(),
                                                ������������� = property.GetValue(newEntry)?.ToString(),
                                                ��� = documentXML.����.�����,
                                                ������������� = DateTime.Now
                                            };

                                            // //_dbcontext.����������������.Add(changes);
                                            changeList.Add(changes);
                                        }
                                    }

                                    lock (listLockers[16])
                                    {
                                        IPSubTables.����������������_Insert.AddRange(changeList);
                                    }
                                }
                            }

                            //_dbcontext.Entry(IPDB).Collection(u => u.�����_��������).Load();

                            //if (IPDB.�����_��������.FirstOrDefault() != null)
                            //{
                            //    var previousEntry = IPDB.�����_��������.FirstOrDefault();

                            //    _dbcontext.Entry(IPDB.�����_��������.FirstOrDefault()).State = EntityState.Deleted;
                            //    IPDB.�����_�������� = new List<EGRIPSvAccountingNO> { new EGRIPSvAccountingNO { Id = IPDB.�����_��������.FirstOrDefault()?.Id != null ? IPDB.�����_��������.FirstOrDefault().Id : Guid.NewGuid().ToString(), ����� = documentXML.����.��������.�����, ���������� = documentXML.����.��������.����������, ������ = documentXML.����.��������.����.������, ����� = documentXML.����.��������.����.������, ���������� = documentXML.����.��������.���������.����������, ����� = documentXML.����.��������.���������.�����, id���� = IPDB.Id } };
                            //    _dbcontext.Entry(IPDB.�����_��������.FirstOrDefault()).State = EntityState.Added;

                            //    List<ChangeLog> changeList = new List<ChangeLog>();

                            //    foreach (var property in typeof(EGRIPSvAccountingNO).GetProperties())
                            //    {
                            //        if (property.GetValue(previousEntry)?.ToString() != property.GetValue(IPDB.�����_��������.FirstOrDefault())?.ToString())
                            //        {
                            //            ChangeLog changes = new ChangeLog();

                            //            changes.������� = "EGRIPSvAccountingNO";
                            //            changes.������� = property.Name;
                            //            changes.���������� = property.GetValue(previousEntry)?.ToString();
                            //            changes.������������� = property.GetValue(IPDB.�����_��������.FirstOrDefault())?.ToString();
                            //            changes.��� = documentXML.����.�����;
                            //            changes.������������� = DateTime.Now;

                            //            //_dbcontext.����������������.Add(changes);
                            //            changeList.Add(changes);
                            //        }
                            //    }

                            //    _dbcontext.����������������.AddRange(changeList);
                            //}
                            //else
                            //{
                            //    IPDB.�����_�������� = new List<EGRIPSvAccountingNO> { new EGRIPSvAccountingNO { Id = Guid.NewGuid().ToString(), ����� = documentXML.����.��������.�����, ���������� = documentXML.����.��������.����������, ������ = documentXML.����.��������.����.������, ����� = documentXML.����.��������.����.������, ���������� = documentXML.����.��������.���������.����������, ����� = documentXML.����.��������.���������.�����, id���� = IPDB.Id } };
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
                //        Console.WriteLine($"�������� ������ ������������ ��������� ���� {documentXML.����.�����}, ����������� ������ ������� ��������");
                //        lock (locker)
                //        {
                //            if (entitiesInWork.Contains(documentXML.����.�����))
                //            {
                //                entitiesInWork.Remove(documentXML.����.�����);
                //            }
                //        }
                //        ParseIPDataDB((object)documentXML);
                //    }
                //}

                //try
                //{
                //    lock (locker)
                //    {
                //        if (entitiesInWork.Contains(documentXML.����.�����))
                //        {
                //            entitiesInWork.Remove(documentXML.����.�����);
                //        }
                //    }                  
                //}
                //catch (Exception e)
                //{

                //}               

                if (docCount - 1 == finishedWorkersCount)
                {
                    _dbcontext.�����_�����.Where(e => IPSubTables.�����_�����_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.�����_�����_Insert);

                    _dbcontext.�����_��������.Where(e => IPSubTables.�����_��������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.�����_��������_Insert);

                    _dbcontext.�����_�������.Where(e => IPSubTables.�����_�������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.�����_�������_Insert);

                    _dbcontext.�����_����.Where(e => IPSubTables.�����_����_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.�����_����_Insert);

                    _dbcontext.�����_��������.Where(e => IPSubTables.�����_��������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.�����_��������_Insert);

                    _dbcontext.�����_�������.Where(e => IPSubTables.�����_�������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.�����_�������_Insert);

                    _dbcontext.�����_����.Where(e => IPSubTables.�����_����_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.�����_����_Insert);

                    _dbcontext.�����_����������.Where(e => IPSubTables.�����_����������_Delete.Contains(e.id����.ToString())).ExecuteDelete();

                    List<EGRIPSvLicense> svLicenseList = new List<EGRIPSvLicense>();
                    foreach (var entries in IPSubTables.�����_����������_Insert)
                    {
                        svLicenseList.Add(entries.Entry);
                    }

                    _dbcontext.BulkCopy(svLicenseList);

                    _dbcontext.�����_���������.Where(e => IPSubTables.�����_���������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.�����_���������_Insert);

                    _dbcontext.�����_��������.Where(e => IPSubTables.�����_��������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.�����_��������_Insert);

                    _dbcontext.�����_�������.Where(e => IPSubTables.�����_�������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.�����_�������_Insert);

                    _dbcontext.�����_�������.Where(e => IPSubTables.�����_�������_Delete.Contains(e.id����.ToString())).ExecuteDelete();
                    _dbcontext.BulkCopy(IPSubTables.�����_�������_Insert);

                    _dbcontext.BulkCopy(IPSubTables.����������������_Insert);

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

            public static List<EGRIPOKVED>? �����_�����_Insert = new List<EGRIPOKVED>();
            public static List<EGRIPSvAccountingNO>? �����_��������_Insert = new List<EGRIPSvAccountingNO>();
            public static List<EGRIPSvAdrMJ>? �����_�������_Insert = new List<EGRIPSvAdrMJ>();
            public static List<EGRIPSVFL>? �����_����_Insert = new List<EGRIPSVFL>();
            public static List<EGRIPSvGegOrg>? �����_��������_Insert = new List<EGRIPSvGegOrg>();
            public static List<EGRIPSvGrajd> �����_�������_Insert = new List<EGRIPSvGrajd>();
            public static List<EGRIPSvIP>? �����_����_Insert = new List<EGRIPSvIP>();
            public static List<(EGRIPSvLicense Entry, DateTime Date)> �����_����������_Insert = new List<(EGRIPSvLicense Entry, DateTime Date)>();
            public static List<EGRIPSvPrekras_> �����_���������_Insert = new List<EGRIPSvPrekras_>();
            public static List<EGRIPSvRegFSS> �����_��������_Insert = new List<EGRIPSvRegFSS>();
            public static List<EGRIPSvRegIP> �����_�������_Insert = new List<EGRIPSvRegIP>();
            public static List<EGRIPSvRegPF> �����_�������_Insert = new List<EGRIPSvRegPF>();

            public static List<string?> �����_�����_Delete = new List<string?>();
            public static List<string?> �����_��������_Delete = new List<string?>();
            public static List<string?> �����_�������_Delete = new List<string?>();
            public static List<string?> �����_����_Delete = new List<string?>();
            public static List<string?> �����_��������_Delete = new List<string?>();
            public static List<string?> �����_�������_Delete = new List<string?>();
            public static List<string?> �����_����_Delete = new List<string?>();
            public static List<string?> �����_����������_Delete = new List<string?>();
            public static List<string?> �����_���������_Delete = new List<string?>();
            public static List<string?> �����_��������_Delete = new List<string?>();
            public static List<string?> �����_�������_Delete = new List<string?>();
            public static List<string?> �����_�������_Delete = new List<string?>();

            public static List<ChangeLog> ����������������_Insert = new List<ChangeLog>();

            public static void ClearIPSubTables()
            {
                IPSubTables.IPInProcessing.Clear();

                IPSubTables.�����_�����_Insert.Clear();
                IPSubTables.�����_�����_Delete.Clear();

                IPSubTables.�����_��������_Insert.Clear();
                IPSubTables.�����_��������_Delete.Clear();

                IPSubTables.�����_�������_Insert.Clear();
                IPSubTables.�����_�������_Delete.Clear();

                IPSubTables.�����_����_Insert.Clear();
                IPSubTables.�����_����_Delete.Clear();

                IPSubTables.�����_��������_Insert.Clear();
                IPSubTables.�����_��������_Delete.Clear();

                IPSubTables.�����_�������_Insert.Clear();
                IPSubTables.�����_�������_Delete.Clear();

                IPSubTables.�����_����_Insert.Clear();
                IPSubTables.�����_����_Delete.Clear();

                IPSubTables.�����_����������_Insert.Clear();
                IPSubTables.�����_����������_Delete.Clear();

                IPSubTables.�����_���������_Insert.Clear();
                IPSubTables.�����_���������_Delete.Clear();

                IPSubTables.�����_��������_Insert.Clear();
                IPSubTables.�����_��������_Delete.Clear();

                IPSubTables.�����_�������_Insert.Clear();
                IPSubTables.�����_�������_Delete.Clear();

                IPSubTables.�����_�������_Insert.Clear();
                IPSubTables.�����_�������_Delete.Clear();

                IPSubTables.����������������_Insert.Clear();
            }
        }

        public static class ULSubTables
        {
            public static List<UL> ULInProcessing = new List<UL>();

            public static List<EGRULOKVED> �����_�����_Insert = new List<EGRULOKVED>();
            public static List<EGRULSvAddressUL> �����_���������_Insert = new List<EGRULSvAddressUL>();
            public static List<EGRULSvDerjRegistryAO> �����_��������������_Insert = new List<EGRULSvDerjRegistryAO>();
            public static List<(EGRULSvShareOOO Entry, DateTime? Date)> �����_���������_Insert = new List<(EGRULSvShareOOO Entry, DateTime? Date)>();
            public static List<EGRULSvZapEGRUL> �����_����������_Insert = new List<EGRULSvZapEGRUL>();
            public static List<EGRULSvLicense> �����_����������_Insert = new List<EGRULSvLicense>();
            public static List<EGRULSvNaimUL> �����_��������_Insert = new List<EGRULSvNaimUL>();
            public static List<EGRULSvObrUL> �����_�������_Insert = new List<EGRULSvObrUL>();
            public static List<EGRULSvPodrazd> �����_���������_Insert = new List<EGRULSvPodrazd>();
            public static List<EGRULSvPredsh> �����_�������_Insert = new List<EGRULSvPredsh>();
            public static List<EGRULSvPreem> �����_�������_Insert = new List<EGRULSvPreem>();
            public static List<EGRULSvPrekrUL> �����_���������_Insert = new List<EGRULSvPrekrUL>();
            public static List<EGRULSvRegOrg> �����_��������_Insert = new List<EGRULSvRegOrg>();
            public static List<EGRULSvRegPF> �����_�������_Insert = new List<EGRULSvRegPF>();
            public static List<EGRULSvRegFSS> �����_��������_Insert = new List<EGRULSvRegFSS>();
            public static List<EGRULSvReorg> �����_�������_Insert = new List<EGRULSvReorg>();
            public static List<EGRULSvStatus> �����_��������_Insert = new List<EGRULSvStatus>();
            public static List<EGRULSvUstKap> �����_��������_Insert = new List<EGRULSvUstKap>();
            public static List<EGRULSvAccountingNO> �����_��������_Insert = new List<EGRULSvAccountingNO>();
            public static List<EGRULSvFounder> �����_���������_Insert = new List<EGRULSvFounder>();
            public static List<EGRULSvUL> �����_����_Insert = new List<EGRULSvUL>();
            public static List<EGRULSvedDoljnFL> �����_�����������_Insert = new List<EGRULSvedDoljnFL>();

            public static List<string?> �����_�����_Delete = new List<string?>();
            public static List<string?> �����_���������_Delete = new List<string?>();
            public static List<string?> �����_��������������_Delete = new List<string?>();
            public static List<string?> �����_���������_Delete = new List<string?>();
            public static List<string?> �����_����������_Delete = new List<string?>();
            public static List<string?> �����_����������_Delete = new List<string?>();
            public static List<string?> �����_��������_Delete = new List<string?>();
            public static List<string?> �����_�������_Delete = new List<string?>();
            public static List<string?> �����_���������_Delete = new List<string?>();
            public static List<string?> �����_�������_Delete = new List<string?>();
            public static List<string?> �����_�������_Delete = new List<string?>();
            public static List<string?> �����_���������_Delete = new List<string?>();
            public static List<string?> �����_��������_Delete = new List<string?>();
            public static List<string?> �����_�������_Delete = new List<string?>();
            public static List<string?> �����_��������_Delete = new List<string?>();
            public static List<string?> �����_�������_Delete = new List<string?>();
            public static List<string?> �����_��������_Delete = new List<string?>();
            public static List<string?> �����_��������_Delete = new List<string?>();
            public static List<string?> �����_��������_Delete = new List<string?>();
            public static List<string?> �����_���������_Delete = new List<string?>();
            public static List<string?> �����_����_Delete = new List<string?>();
            public static List<string?> �����_�����������_Delete = new List<string?>();

            public static void ClearULSubTables()
            {
                ULSubTables.ULInProcessing.Clear();

                ULSubTables.�����_�����_Insert.Clear();
                ULSubTables.�����_�����_Delete.Clear();

                ULSubTables.�����_���������_Insert.Clear();
                ULSubTables.�����_���������_Delete.Clear();

                ULSubTables.�����_��������������_Insert.Clear();
                ULSubTables.�����_��������������_Delete.Clear();

                ULSubTables.�����_���������_Insert.Clear();
                ULSubTables.�����_���������_Delete.Clear();

                ULSubTables.�����_�����������_Insert.Clear();
                ULSubTables.�����_�����������_Delete.Clear();

                ULSubTables.�����_����������_Insert.Clear();
                ULSubTables.�����_����������_Delete.Clear();

                ULSubTables.�����_����������_Insert.Clear();
                ULSubTables.�����_����������_Delete.Clear();

                ULSubTables.�����_��������_Insert.Clear();
                ULSubTables.�����_��������_Delete.Clear();

                ULSubTables.�����_�������_Insert.Clear();
                ULSubTables.�����_�������_Delete.Clear();

                ULSubTables.�����_���������_Insert.Clear();
                ULSubTables.�����_���������_Delete.Clear();

                ULSubTables.�����_�������_Insert.Clear();
                ULSubTables.�����_�������_Delete.Clear();

                ULSubTables.�����_�������_Insert.Clear();
                ULSubTables.�����_�������_Delete.Clear();

                ULSubTables.�����_���������_Insert.Clear();
                ULSubTables.�����_���������_Delete.Clear();

                ULSubTables.�����_��������_Insert.Clear();
                ULSubTables.�����_��������_Delete.Clear();

                ULSubTables.�����_�������_Insert.Clear();
                ULSubTables.�����_�������_Delete.Clear();

                ULSubTables.�����_��������_Insert.Clear();
                ULSubTables.�����_��������_Delete.Clear();

                ULSubTables.�����_�������_Insert.Clear();
                ULSubTables.�����_�������_Delete.Clear();

                ULSubTables.�����_��������_Insert.Clear();
                ULSubTables.�����_��������_Delete.Clear();

                ULSubTables.�����_��������_Insert.Clear();
                ULSubTables.�����_��������_Delete.Clear();

                ULSubTables.�����_��������_Insert.Clear();
                ULSubTables.�����_��������_Delete.Clear();

                ULSubTables.�����_���������_Insert.Clear();
                ULSubTables.�����_���������_Delete.Clear();

                ULSubTables.�����_����_Insert.Clear();
                ULSubTables.�����_����_Delete.Clear();

                IPSubTables.����������������_Insert.Clear();
            }
        }
    }

}
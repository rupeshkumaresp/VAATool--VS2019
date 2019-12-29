using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using VAA.BusinessComponents.ChiliWebService;
using VAA.BusinessComponents.Interfaces;
using VAA.DataAccess;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.BusinessComponents
{
    /// <summary>
    /// Chili processing engine
    /// Resposible for Chili proof, PDF, Tasks
    /// </summary>
    public class ChiliProcessor : IChiliProcessor
    {
        private readonly VAAEntities _context = new VAAEntities();
        MenuManagement _menuManagement = new MenuManagement();

        public string Environment { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
        public mainSoapClient WebService { get; set; }

        public ChiliProcessor()
        {
            WebService = new mainSoapClient();
            Connect("VAA");
        }

        public void Connect(string environment)
        {
            Connect(environment, "WebServices", "C@ps1cum123ws");
        }

        public void Connect(string environment, string username, string password)
        {
            if (environment == Environment && username == Username && password == Password) return; // Don't reconnect if not needed
            this.Environment = environment;
            this.Username = username;
            this.Password = password;
            var generateApiKeyResponse = new XmlDocument();
            generateApiKeyResponse.LoadXml(WebService.GenerateApiKey(environment, "TestUser", "TestUser"));
            if (generateApiKeyResponse.DocumentElement != null && generateApiKeyResponse.DocumentElement.GetAttribute("succeeded") == "true")
                ApiKey = generateApiKeyResponse.DocumentElement.GetAttribute("key");
        }

        #region ForMenu

        public string CloneChiliDocument(long menuId)
        {
            string relativeDocPath = Convert.ToString(menuId);

            var baseChiliDocument = _context.tMenuTemplates.Where(m => m.MenuID == menuId).Select(m => m.ChiliDocumentID).FirstOrDefault();


            Connect(Environment);

            var copyItemResult = new XmlDocument();
            copyItemResult.LoadXml(WebService.ResourceItemCopy(ApiKey, "documents", baseChiliDocument, menuId.ToString(), "MergeIt/_ACTIVE_/" + relativeDocPath));
            var newDocumentID = copyItemResult.DocumentElement.GetAttribute("id");

            return newDocumentID;
        }

        public string CreateChiliDocumentForMenu(int instanceId, long menuId, int templateId, string docName)
        {
            string relativeDocPath = Convert.ToString(menuId);

            var baseDocumentID = (from t in this._context.tTemplates where t.TemplateID == templateId select t.ChiliDocumentID).FirstOrDefault();

            if (string.IsNullOrEmpty(baseDocumentID)) throw new Exception("No base Chili document ID specified for this template");

            var templateName = (from t in _context.tTemplates where t.TemplateID == templateId select t.Name).FirstOrDefault();

            Connect(Environment);

            var copyItemResult = new XmlDocument();
            copyItemResult.LoadXml(WebService.ResourceItemCopy(ApiKey, "documents", baseDocumentID, docName, "MergeIt/_ACTIVE_/" + relativeDocPath));
            var newDocumentID = copyItemResult.DocumentElement.GetAttribute("id");
            // TODO catch error here
            return newDocumentID;
        }

        public string CreateChiliDocumentForReOrderMenu(int instanceId, long menuId, string oldChiliDocId, string docName)
        {
            string relativeDocPath = Convert.ToString(menuId);

            var baseDocumentID = oldChiliDocId;
            if (string.IsNullOrEmpty(baseDocumentID)) throw new Exception("No base Chili document ID specified for this template");


            Connect(Environment);

            var copyItemResult = new XmlDocument();
            copyItemResult.LoadXml(WebService.ResourceItemCopy(ApiKey, "documents", baseDocumentID, docName, "MergeIt/_ACTIVE_/" + relativeDocPath));
            var newDocumentID = copyItemResult.DocumentElement.GetAttribute("id");
            // TODO catch error here
            return newDocumentID;
        }

        public void UpdateAllChiliDocuments(int instanceId, int cycleId)
        {
            var menus = _menuManagement.GetMenuByCycle(cycleId).ToList();

            foreach (var menu in menus)
            {
                UpdateChiliDocumentVariables(instanceId, menu.Id);
            }

        }


        public void UpdateChiliDocumentVariablesallowServerRendering(int instanceId, long menuId, string chiliDocumentId)
        {

            var menuTemplate = _menuManagement.GetMenuTemplate(menuId);

            Dictionary<string, string> values = new Dictionary<string, string>();

            var templateProcessorClasses = new string[] { "VAA.BusinessComponents.VAATemplateProcessor" };

            try
            {
                Connect(Environment);

                var theseValues = new Dictionary<string, string>(values);

                // Run custom template processing code
                if (templateProcessorClasses != null && templateProcessorClasses.Length > 0)
                {
                    var document = new XmlDocument();
                    document.LoadXml(WebService.ResourceItemGetXML(ApiKey, "documents", chiliDocumentId));
                    foreach (var templateProcessorClass in templateProcessorClasses)
                    {
                        var templateProcessor = (ITemplateProcessor)Activator.CreateInstance(null, templateProcessorClass).Unwrap();
                        templateProcessor.ProcessDocument(document, theseValues);
                    }
                    WebService.ResourceItemSave(ApiKey, "documents", chiliDocumentId, document.OuterXml);
                }

                var variablesXml = new XElement("variables", new XAttribute("savedInEditor", "false"), new XAttribute("allowServerRendering", "true")); // savedInEditor=false doesn't seem to have any effect?
                //var variablesXml = new XElement("variables"); // savedInEditor=false doesn't seem to have any effect?
                foreach (var valueKvp in theseValues)
                {
                    variablesXml.Add(new XElement("item", new XAttribute("name", valueKvp.Key), new XAttribute("value", valueKvp.Value ?? "")));
                }
                WebService.DocumentSetVariableValues(ApiKey, chiliDocumentId, variablesXml.ToString());
            }

            catch { }


        }
        public void UpdateChiliDocumentVariables(int instanceId, long menuId)
        {
            var menuTemplate = _menuManagement.GetMenuTemplate(menuId);

            Dictionary<string, string> values = new Dictionary<string, string>();// GetFieldsAndValuesDict(menuID); 

            UpdateChiliDocumentVariables(instanceId, menuId, values);
        }

        public void UpdateChiliDocumentVariables(int instanceId, long menuId, Dictionary<string, string> values)
        {
            var menuTemplates = _menuManagement.GetMenuTemplate(menuId);

            UpdateChiliDocumentVariables(instanceId, menuTemplates.ChiliDocumentID, new string[] { "VAA.BusinessComponents.VAATemplateProcessor" }, values);
        }

        public void UpdateChiliDocumentVariables(int instanceId, string chiliDocumentId, string[] templateProcessorClasses,
            Dictionary<string, string> values)
        {
            try
            {
                Connect(Environment);

                var theseValues = new Dictionary<string, string>(values);

                // Run custom template processing code
                if (templateProcessorClasses != null && templateProcessorClasses.Length > 0)
                {
                    var document = new XmlDocument();
                    document.LoadXml(WebService.ResourceItemGetXML(ApiKey, "documents", chiliDocumentId));
                    foreach (var templateProcessorClass in templateProcessorClasses)
                    {
                        var templateProcessor = (ITemplateProcessor)Activator.CreateInstance(null, templateProcessorClass).Unwrap();
                        templateProcessor.ProcessDocument(document, theseValues);
                    }
                    WebService.ResourceItemSave(ApiKey, "documents", chiliDocumentId, document.OuterXml);
                }

                var variablesXml = new XElement("variables", new XAttribute("savedInEditor", "false")); // savedInEditor=false doesn't seem to have any effect?
                //var variablesXml = new XElement("variables"); // savedInEditor=false doesn't seem to have any effect?
                foreach (var valueKvp in theseValues)
                {
                    variablesXml.Add(new XElement("item", new XAttribute("name", valueKvp.Key), new XAttribute("value", valueKvp.Value ?? "")));
                }
                WebService.DocumentSetVariableValues(ApiKey, chiliDocumentId, variablesXml.ToString());
            }

            catch { }
        }


        public void RebuildAllChiliDocuments(int instanceId, int cycleId, int? templateId)
        {
            var menus = _menuManagement.GetMenuByCycle(cycleId);

            foreach (var menu in menus)
            {
                var menuId = menu.Id;
                var menuTemplate = _menuManagement.GetMenuTemplate(menuId);
                RebuildChiliDocument(instanceId, menuId, menuTemplate.TemplateID);
            }
        }

        public void RebuildChiliDocument(int instanceId, long menuId, int? templateId)
        {
            Connect(Environment);

            var menuTemplate = _menuManagement.GetMenuTemplate(menuId);
            var path = Convert.ToString(menuId);

            try
            {
                // Create new Chili document
                var oldDocumentID = menuTemplate.ChiliDocumentID;
                var newChiliDocumentID = CreateChiliDocumentForMenu(instanceId, menuId, menuTemplate.TemplateID, path);

                // Link it to the existing RecordTemplate row
                menuTemplate.ChiliDocumentID = newChiliDocumentID;
                _context.SaveChanges();

                // Delete the old Chili document
                try
                {
                    WebService.ResourceItemDelete(ApiKey, "documents", oldDocumentID);
                }
                catch (Exception)
                {
                }
            }
            catch { }

            UpdateChiliDocumentVariables(instanceId, menuId);
        }

        public void GetLatestPdfGenerationJobId(int instanceId)
        {
            throw new System.NotImplementedException();
        }

        public int CreatePdfGenerationJob(int instanceId, long menuId)
        {
            var efJob = new PdfGenerationJob
            {
                Date = DateTime.Now
            };

            _context.SaveChanges();


            var menutemplate = _menuManagement.GetMenuTemplate(menuId);


            var efTask = new PdfGenerationTask
            {
                MenuId = menuId,
                TemplateId = menutemplate.TemplateID,
                Status = "Ready",
                ChiliDocumentId = menutemplate.ChiliDocumentID,
                ChiliError = null,
                ChiliPdfurl = null,
                ChiliTaskId = null,
                PdfGenerationJobId = efJob.Id
            };

            return efJob.Id;
        }


        public void AddPdfGenerationTaskForMenuTemplate(tMenuTemplates menuTemplate)
        {
            //DO NOT DO PDF GENERATION FOR LANGUAGE MENU AS THEY DO NOT WORK WITH CHILI
            var menu = _menuManagement.GetMenuById(menuTemplate.MenuID);

            //if (menu.LanguageId == 1)
            {
                bool overwriteExisting = true;

                var efTask = (from t in _context.tPDFGenerationTasks
                              where t.PDFGenerationJobID == 1 && t.MenuID == menuTemplate.MenuID && t.TemplateID == menuTemplate.TemplateID
                              select t).FirstOrDefault();

                if (efTask == null || overwriteExisting)
                {
                    if (overwriteExisting && efTask != null)
                    {
                        _context.tPDFGenerationTasks.Remove(efTask);
                    }
                    efTask = new tPDFGenerationTasks
                    {
                        PDFGenerationJobID = 1,
                        MenuID = menuTemplate.MenuID,
                        TemplateID = menuTemplate.TemplateID,
                        Status = "Ready",
                        ChiliDocumentID = menuTemplate.ChiliDocumentID,
                        ChiliError = null,
                        ChiliPDFURL = null,
                        ChiliTaskID = null
                    };
                    _context.tPDFGenerationTasks.Add(efTask);
                    _context.SaveChanges();
                }
            }
        }

        public void AddPdfGenerationTaskForMenuTemplate(tMenuTemplates menuTemplate, string chiliDocId)
        {
            //DO NOT DO PDF GENERATION FOR LANGUAGE MENU AS THEY DO NOT WORK WITH CHILI
            var menu = _menuManagement.GetMenuById(menuTemplate.MenuID);

            //if (menu.LanguageId == 1)
            {
                bool overwriteExisting = true;

                var efTask = (from t in _context.tPDFGenerationTasks
                              where t.PDFGenerationJobID == 1 && t.MenuID == menuTemplate.MenuID && t.TemplateID == menuTemplate.TemplateID
                              select t).FirstOrDefault();

                if (efTask == null || overwriteExisting)
                {
                    if (overwriteExisting && efTask != null)
                    {
                        _context.tPDFGenerationTasks.Remove(efTask);
                    }
                    efTask = new tPDFGenerationTasks
                    {
                        PDFGenerationJobID = 1,
                        MenuID = menuTemplate.MenuID,
                        TemplateID = menuTemplate.TemplateID,
                        Status = "Ready",
                        ChiliDocumentID = chiliDocId,
                        ChiliError = null,
                        ChiliPDFURL = null,
                        ChiliTaskID = null
                    };
                    _context.tPDFGenerationTasks.Add(efTask);
                    _context.SaveChanges();
                }
            }
        }

        public PdfGenerationJob GetPdfGenerationJob(int jobId)
        {
            var efJob = (from j in _context.tPDFGenerationJobs
                         where j.PDFGenerationJobID == jobId

                         select new PdfGenerationJob
                         {
                             Id = jobId,
                             Date = j.Date,
                             Tasks = (from t in j.tPDFGenerationTasks
                                      select new PdfGenerationTask
                                      {
                                          PdfGenerationJobId = t.PDFGenerationJobID,
                                          MenuId = t.MenuID,
                                          TemplateId = t.TemplateID,
                                          Status = t.Status,
                                          MergeQuantity = t.MergeQuantity,
                                          ChiliDocumentId = t.ChiliDocumentID,
                                          ChiliError = t.ChiliError,
                                          ChiliPdfurl = t.ChiliPDFURL,
                                          ChiliTaskId = t.ChiliTaskID,
                                          LocalPdfFile = t.LocalPDFFile
                                      })
                         }

                         ).FirstOrDefault();
            return efJob;
        }

        public PdfGenerationTask GetPdfGenerationTask(int jobId, long menuId)
        {
            var efTask = (from t in _context.tPDFGenerationTasks where t.PDFGenerationJobID == jobId && t.MenuID == menuId select t).FirstOrDefault();

            if (efTask == null) return null;

            return new PdfGenerationTask
            {
                PdfGenerationJobId = efTask.PDFGenerationJobID,
                MenuId = efTask.MenuID,
                TemplateId = efTask.TemplateID,
                //CategoryID = efTask.Record.CategoryID,
                Status = efTask.Status,
                MergeQuantity = efTask.MergeQuantity,
                ChiliDocumentId = efTask.ChiliDocumentID,
                ChiliError = efTask.ChiliError,
                ChiliPdfurl = efTask.ChiliPDFURL,
                ChiliTaskId = efTask.ChiliTaskID,
                LocalPdfFile = efTask.LocalPDFFile
            };
        }

        public void UpdateTask(int jobId, long menuId, int templateId, string status, string error)
        {
            var efTask = (from t in _context.tPDFGenerationTasks
                          where t.PDFGenerationJobID == jobId && t.MenuID == menuId && t.TemplateID == templateId
                          select t).FirstOrDefault();
            efTask.ChiliError = error;
            efTask.Status = status;
            _context.SaveChanges();
        }

        public void UpdateTask(PdfGenerationTask task)
        {
            var efTask = (from t in _context.tPDFGenerationTasks
                          where t.PDFGenerationJobID == task.PdfGenerationJobId && t.MenuID == task.MenuId && t.TemplateID == task.TemplateId
                          select t).FirstOrDefault();
            efTask.ChiliDocumentID = task.ChiliDocumentId;
            efTask.ChiliError = task.ChiliError;
            efTask.ChiliPDFURL = task.ChiliPdfurl;
            efTask.ChiliTaskID = task.ChiliTaskId;
            efTask.Status = task.Status;
            efTask.LocalPDFFile = task.LocalPdfFile;
            _context.SaveChanges();
        }

        #endregion

        #region PackingTicket


        public void UpdateChiliDocumentVariablesBoxTickets(int instanceId, long boxTicketId, Dictionary<string, string> values)
        {
            var boxTicketTemplate = _menuManagement.GetBoxTicketTemplate(boxTicketId);

            UpdateChiliDocumentVariables(instanceId, boxTicketTemplate.ChiliDocumentID, new string[] { "VAA.BusinessComponents.VAATemplateProcessor" }, values);
        }



        public string CreateChiliDocumentForPackingTicket(int instanceId, long boxTicketId, int templateId, string docName)
        {
            string relativeDocPath = Convert.ToString(boxTicketId);

            var baseDocumentID = (from t in this._context.tTemplatesPacking where t.PackingTemplateID == templateId select t.ChiliDocumentID).FirstOrDefault();

            if (string.IsNullOrEmpty(baseDocumentID)) throw new Exception("No base Chili document ID specified for this template");

            var templateName = (from t in _context.tTemplatesPacking where t.PackingTemplateID == templateId select t.Name).FirstOrDefault();

            Connect(Environment);

            var copyItemResult = new XmlDocument();
            copyItemResult.LoadXml(WebService.ResourceItemCopy(ApiKey, "documents", baseDocumentID, docName, "MergeIt/_ACTIVE_/BoxTicket" + relativeDocPath));
            var newDocumentID = copyItemResult.DocumentElement.GetAttribute("id");
            // TODO catch error here
            return newDocumentID;
        }


        public tPDFGenerationTasksPackingTicket AddPdfGenerationTaskForBoxTicketTemplate(tBoxTicketTemplate boxTicketTemplate)
        {
            bool overwriteExisting = true;

            var efTask = (from t in _context.tPDFGenerationTasksPackingTicket where t.PDFGenerationJobID == 1 && t.BoxTicketID == boxTicketTemplate.BoxTicketID && t.PackingTemplateID == boxTicketTemplate.TemplateID select t).FirstOrDefault();

            if (efTask == null || overwriteExisting)
            {
                if (overwriteExisting && efTask != null)
                {
                    _context.tPDFGenerationTasksPackingTicket.Remove(efTask);
                }
                efTask = new tPDFGenerationTasksPackingTicket
                {
                    PDFGenerationJobID = 1,
                    BoxTicketID = boxTicketTemplate.BoxTicketID,
                    PackingTemplateID = boxTicketTemplate.TemplateID,
                    Status = "Ready",
                    ChiliDocumentID = boxTicketTemplate.ChiliDocumentID,
                    ChiliError = null,
                    ChiliPDFURL = null,
                    ChiliTaskID = null
                };
                _context.tPDFGenerationTasksPackingTicket.Add(efTask);
                _context.SaveChanges();
            }

            return efTask;
        }


        public PdfGenerationJobPackingTicket GetPdfGenerationJobPackingTicket(int jobId)
        {
            var efJob = (from j in _context.tPDFGenerationJobs
                         where j.PDFGenerationJobID == jobId

                         select new PdfGenerationJobPackingTicket
                         {
                             Id = jobId,
                             Date = j.Date,
                             Tasks = (from t in j.tPDFGenerationTasksPackingTicket
                                      select new PdfGenerationTaskPackingTicket
                                      {
                                          PdfGenerationJobId = t.PDFGenerationJobID,
                                          BoxTicketId = t.BoxTicketID,
                                          TemplateId = t.PackingTemplateID,
                                          Status = t.Status,
                                          MergeQuantity = t.MergeQuantity,
                                          ChiliDocumentId = t.ChiliDocumentID,
                                          ChiliError = t.ChiliError,
                                          ChiliPdfurl = t.ChiliPDFURL,
                                          ChiliTaskId = t.ChiliTaskID,
                                          LocalPdfFile = t.LocalPDFFile
                                      })

                         }

                         ).FirstOrDefault();
            return efJob;
        }

        public PdfGenerationTaskPackingTicket GetPdfGenerationTaskPackingTicket(int jobId, long boxTicketId)
        {
            var efTask = (from t in _context.tPDFGenerationTasksPackingTicket where t.PDFGenerationJobID == jobId && t.BoxTicketID == boxTicketId select t).FirstOrDefault();

            if (efTask == null) return null;

            return new PdfGenerationTaskPackingTicket
            {
                PdfGenerationJobId = efTask.PDFGenerationJobID,
                BoxTicketId = efTask.BoxTicketID,
                TemplateId = efTask.PackingTemplateID,
                //CategoryID = efTask.Record.CategoryID,
                Status = efTask.Status,
                MergeQuantity = efTask.MergeQuantity,
                ChiliDocumentId = efTask.ChiliDocumentID,
                ChiliError = efTask.ChiliError,
                ChiliPdfurl = efTask.ChiliPDFURL,
                ChiliTaskId = efTask.ChiliTaskID,
                LocalPdfFile = efTask.LocalPDFFile
            };
        }

        public void UpdateTaskPackingTicket(int jobId, long boxTicketId, int templateId, string status, string error)
        {
            var efTask = (from t in _context.tPDFGenerationTasksPackingTicket
                          where t.PDFGenerationJobID == jobId && t.BoxTicketID == boxTicketId && t.PackingTemplateID == templateId
                          select t).FirstOrDefault();
            efTask.ChiliError = error;
            efTask.Status = status;
            _context.SaveChanges();
        }

        public void UpdateTaskPackingTicket(PdfGenerationTaskPackingTicket task)
        {
            var efTask = (from t in _context.tPDFGenerationTasksPackingTicket
                          where t.PDFGenerationJobID == task.PdfGenerationJobId && t.BoxTicketID == task.BoxTicketId && t.PackingTemplateID == task.TemplateId
                          select t).FirstOrDefault();
            efTask.ChiliDocumentID = task.ChiliDocumentId;
            efTask.ChiliError = task.ChiliError;
            efTask.ChiliPDFURL = task.ChiliPdfurl;
            efTask.ChiliTaskID = task.ChiliTaskId;
            efTask.Status = task.Status;
            efTask.LocalPDFFile = task.LocalPdfFile;
            _context.SaveChanges();
        }

        #endregion

    }
}

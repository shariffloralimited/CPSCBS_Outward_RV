using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Configuration;
using System.Timers;
using System.Xml;
using System.Threading;
using System.Net;

namespace CPSCBS
{
    public partial class Service1 : ServiceBase
    {
        static System.Timers.Timer timer1= new System.Timers.Timer(5000);

        public Service1()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            if (!Directory.Exists(AppVariable.LogPath))
            {
                Directory.CreateDirectory(AppVariable.LogPath);
            }
            WriteLog("Service Started.");
            long delay = 5000;
            try
            {
                delay = Int32.Parse(ConfigurationManager.AppSettings["IntervalInSeconds"]) * 1000;
            }
            catch
            {
                WriteLog("IntervalInSeconds key/value incorrect. Default set to 5 secs.");
            }

            if (delay < 5000)
            {

                WriteLog("Sleep time too short: Changed to default(5 secs).");
                delay = 5000;
            }

            timer1.Enabled = true;

            timer1.Start();
            timer1.Interval = delay;

            timer1.Elapsed += new ElapsedEventHandler(OnElapsedTime);

        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            timer1.Stop();
            WriteLog("Waking up...");

            try
            {
                ProcessTransaction();
            }
            catch (Exception ex)
            {
                WriteLog("Error Processing Transaction: " + ex.Message);
            }
            WriteLog("Going to sleep...");
            timer1.Start();
        }
        private void ProcessTransaction()
        {
            CPSDB db = new CPSDB();

            DataTable dt = db.GetOutwardTransactionList("RV");

            ProcessOutward(dt);
        }
        protected override void OnStop()
        {
            WriteLog("Service Stopped.");
        }
        protected void WriteLog(string Msg)
        {
            FileStream fs = new FileStream(AppVariable.LogPath + "\\CBS-Outward-RV-" + System.DateTime.Today.ToString("yyyyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(System.DateTime.Now.ToString() + ": " + Msg);
            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }
        public string XML(string XREF)
        {
            string str = "<S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
             "<S:Body>" +
            "<QUERYTRANSACTION_IOFS_REQ xmlns=\"http://fcubs.ofss.com/service/FCUBSCGService\">" +
           "<FCUBS_HEADER>" +
           "<SOURCE>" + AppVariable.SOURCE + "</SOURCE>" +
           "<UBSCOMP>" + AppVariable.UBSCOMP + "</UBSCOMP>" +
           "<MSGID/>" +
           "<CORRELID/>" +
           "<USERID>" + AppVariable.USERID + "</USERID>" +
           "<BRANCH>" + AppVariable.BRANCH + "</BRANCH>" +
           "<MODULEID>" + AppVariable.MODULEID + "</MODULEID>" +
           "<SERVICE>" + AppVariable.SERVICE + "</SERVICE>" +
           "<OPERATION>" + AppVariable.OPERATION + "</OPERATION>" +
           "<SOURCE_OPERATION>" + AppVariable.SOURCE_OPERATION + "</SOURCE_OPERATION>" +
           "<SOURCE_USERID/>" +
           "<DESTINATION/>" +
           "<MULTITRIPID/>" +
           "<FUNCTIONID/>" +
           "<ACTION/>" +
           "<MSGSTAT/>" +
           "<PASSWORD/>" +
           "<ADDL>" +
           "<PARAM>" +
           "<NAME/>" +
           "<VALUE/>" +
           "</PARAM>" +
           "</ADDL>" +
           "</FCUBS_HEADER>" +
           "<FCUBS_BODY>" +
           "<Transaction-Details>" +
           "<XREF>" + XREF + "</XREF>" +
           "<Clearing-Details>" +
           "<FCCREF></FCCREF>" +
           "</Clearing-Details>" +
           "</Transaction-Details>" +
           "</FCUBS_BODY>" +
           "</QUERYTRANSACTION_IOFS_REQ>" +
           "</S:Body>" +
           "</S:Envelope>";
            return str;
        }
        public string QueryTransaction(string XREF)
        {
            WriteLog("Input (QueryTransaction): XREF =" + XREF);
            string result = "";
            WebRequest req = null;
            WebResponse rsp = null;
            try
            {
                string uri = AppVariable.CBSURI + "/FCUBSCGService/FCUBSCGService?WSDL";
                req = (HttpWebRequest)WebRequest.Create(uri);
                req.Method = "POST";
                req.ContentType = "text/xml";
                req.Credentials = CredentialCache.DefaultNetworkCredentials;
                StreamWriter writer = new StreamWriter(req.GetRequestStream());
                writer.WriteLine(XML(XREF));
                writer.Close();
                rsp = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(rsp.GetResponseStream());
                result = sr.ReadToEnd();
                sr.Close();
            }
            catch (WebException webEx)
            {
                System.Console.WriteLine(webEx.Message.ToString());
                System.Console.WriteLine(webEx.StackTrace.ToString());

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message.ToString());
                System.Console.WriteLine(ex.StackTrace.ToString());
            }
            finally
            {
                if (req != null) req.GetRequestStream().Close();
                if (rsp != null) rsp.GetResponseStream().Close();
            }
           // WriteLog("Output (QueryTransaction)---------------" + result);
            return result;
        }
        private void ProcessOutward(DataTable table)
        {
            CPSDB db = new CPSDB();

            int n = table.Rows.Count;
            for (int i = 0; i < n; i++)
            {
                CBSData data = new CBSData();

                data.Envelop = table.Rows[i]["Envelop"].ToString();
                data.CheckID = table.Rows[i]["CheckID"].ToString();
                data.XREF = table.Rows[i]["XREF"].ToString();
                data.ValDate = table.Rows[i]["ValDate"].ToString();
                data.RemAccount = table.Rows[i]["RemAccount"].ToString();
                data.BenefAccount = table.Rows[i]["BenefAccount"].ToString();
                data.Amount = ((decimal)table.Rows[i]["Amount"]).ToString();
                data.CCY = table.Rows[i]["CCY"].ToString();
                data.HV = table.Rows[i]["HV"].ToString();
                data.OrderSeq = table.Rows[i]["OrderSeq"].ToString();
                data.TrCd = table.Rows[i]["TrCd"].ToString();
                bool NoCharge = false;
                if (data.TrCd == "31" || data.TrCd == "32" || data.TrCd == "99")
                {
                    NoCharge = true;
                }
                data.ProductCode = GetProductCode(data.CCY, "OU", data.OrderSeq, NoCharge);

                //data.UtilityBill = table.Rows[i]["NoCharge"].ToString();
                data.UtilityBill = "FALSE";
                data.Branch = table.Rows[i]["Branch"].ToString();
                data.InstDate = table.Rows[i]["InstDate"].ToString();
                data.ReasonCode = table.Rows[i]["ReasonCode"].ToString();
                data.RejectReason = table.Rows[i]["RejectReason"].ToString();
                data.RemBranch = table.Rows[i]["RemBranch"].ToString();
                data.ACCCYAMT = table.Rows[i]["Amount"].ToString();
                data.RoutingNo = table.Rows[i]["RoutingNo"].ToString();
                data.CheckSlNo = table.Rows[i]["SerialNo"].ToString();
                data.TxnBrn = table.Rows[i]["TxnBrn"].ToString();
                data.TxnDate = table.Rows[i]["TxnDate"].ToString();
                data.InstType = table.Rows[i]["InstType"].ToString();

                WriteLog("CheckID: " + data.CheckID);
                WriteLog("XREF: " + data.XREF);
                WriteLog("ValueDate: " + data.ValDate);
                WriteLog("Amount: " + data.Amount);
                WriteLog("Currency: " + data.CCY);
                WriteLog("RemAccount: " + data.RemAccount);
                WriteLog("BenefAccount: " + data.BenefAccount);

                WriteLog("CheckSlNo: " + data.CheckSlNo);
                WriteLog("RoutingNo: " + data.RoutingNo);
                WriteLog("TxnBrn: " + data.TxnBrn);
                WriteLog("InstType: " + data.InstType);
                WriteLog("TrCd: " + data.TrCd);


                if (data.CheckID != "")
                {
                    WriteLog("Calling CBS Outward: " + data.CheckID);
                    string responsestring = OutwardTransaction(data.Branch, data.XREF, data.HV, data.RemAccount,
                        data.BenefAccount, data.CheckSlNo, data.CCY, data.Amount,
                        data.TxnDate, data.ValDate, data.InstDate, data.RoutingNo, data.TxnBrn, data.RemBranch,
                        data.InstType, data.ReasonCode, data.RejectReason, data.Envelop, data.ProductCode, data.UtilityBill);

                    WriteLog("Calling CBS Outward end");

                    data = ReadResponse(data, responsestring);
                    WriteLog("End Process OutwardTransaction");

                    WriteLog("XREF: " + data.XREF);
                    WriteLog("FCCRef: " + data.FCCRef);
                    WriteLog("MsgId: " + data.MsgID);
                    WriteLog("Status: " + data.Status);
                    WriteLog("CBSResponse: " + data.CBSResponse);

                    WriteLog("Start Processing Query Transaction For " + data.Envelop);
                    try
                    {
                        string result = QueryTransaction(data.XREF);
                        UpdateByQueryTransactionResponse(result, data.Envelop, data.CheckID);
                        WriteLog("data.Envelop" + data.Envelop);

                        WriteLog("End Processing Query Transaction For " + data.Envelop);
                    }
                    catch
                    {

                    }

                    WriteLog("" + data.XREF);
                    WriteLog("" + data.Status);
                    WriteLog("" + data.MsgID);
                    WriteLog("" + data.Envelop);

                    db.UpdateOutwardStatus(data);

                    WriteLog("End Processing Status Update");


                }

                data = null;
            }
            table.Dispose();
        }
        private string GetProductCode(string cCY, string clearingType, string session,bool noCharge)
        {
            CPSDB db = new CPSDB();
            WriteLog("Inside Product Code Method");
            WriteLog("CCY"+ cCY);
            WriteLog("clearingType" + clearingType);
            WriteLog("session" + session);
            WriteLog("noCharge" + noCharge);
            return db.GetProductCode(cCY, clearingType, session, noCharge);
        }
        public void UpdateByQueryTransactionResponse(string CBSResult, string Type, string CheckId)
        {
            // WriteLog("Inside  ----------- UpdateByQueryTransactionResponse");
            XmlDocument rr = new XmlDocument();
            CPSDB db = new CPSDB();

            string QueryTransaction_Status = "";
            string WCODE = "";
            string WDESC = "";
            string XREF = "";
            string FCCREF = "";
            string INSTRNO = "";
            string REMACCOUNT = "";
            string ROUTINGNO = "";
            try
            {
                rr.LoadXml(CBSResult);
                // WriteLog("XML Loaded");
                QueryTransaction_Status = rr.GetElementsByTagName("WDESC").Item(0).InnerText;
                //  WriteLog("QueryTransaction_Status");

            }
            catch
            {

            }


            if (rr.GetElementsByTagName("XREF").Count > 0)
            {
                XREF = rr.GetElementsByTagName("XREF").Item(0).InnerText;
                //     WriteLog("XREF: " + XREF);
            }

            if (Type == "OCE")
            {

                    try
                    {
                        WCODE = rr.GetElementsByTagName("WCODE").Item(0).InnerText;
                    }
                    catch { }

                    try
                    {
                        WDESC = rr.GetElementsByTagName("WDESC").Item(0).InnerText;
                    }
                    catch { }

                    QueryTransaction_Status = WCODE + " " + WDESC;
                    //         WriteLog("QueryTransaction_Status :" + QueryTransaction_Status);

                    XmlNodeList Clearing_Details = rr.GetElementsByTagName("Clearing-Details");
                    foreach (XmlNode node in Clearing_Details)
                    {
                        XmlElement Clearing_DetailsElement = (XmlElement)node;
                        FCCREF = Clearing_DetailsElement.GetElementsByTagName("FCCREF")[0].InnerText;
                        try
                        {
                            INSTRNO = Clearing_DetailsElement.GetElementsByTagName("INSTRNO")[0].InnerText;
                        }
                        catch { }
                        try
                        {
                            REMACCOUNT = Clearing_DetailsElement.GetElementsByTagName("REMACCOUNT")[0].InnerText;
                        }
                        catch { }
                        try
                        {
                            ROUTINGNO = Clearing_DetailsElement.GetElementsByTagName("ROUTINGNO")[0].InnerText;
                        }
                        catch { }


                        //WriteLog("Inside Query Transaction");
                        //WriteLog("FCCREF :" + FCCREF);
                        //WriteLog("INSTRNO  :" + INSTRNO);
                        //WriteLog("REMACCOUNT  :" + REMACCOUNT);
                        //WriteLog("ROUTINGNO  :" + ROUTINGNO);

                        db.InsertTransactionReffOfAOCE(XREF, QueryTransaction_Status, FCCREF, INSTRNO, REMACCOUNT, ROUTINGNO, CheckId);

                        //WriteLog("Inside Query Transaction-----END----");
                    } 
            }
        }
        private CBSData ReadResponse(CBSData data, string responsestring)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(responsestring);

            if (doc.GetElementsByTagName("MSGID").Count > 0)
            {
                data.MsgID = doc.GetElementsByTagName("MSGID").Item(0).InnerText;
                WriteLog("MSGID: " + data.MsgID);
            }
            if (doc.GetElementsByTagName("XREF").Count > 0)
            {
                data.XREF = doc.GetElementsByTagName("XREF").Item(0).InnerText;
                WriteLog("XREF: " + data.XREF);
            }
            if (doc.GetElementsByTagName("FCCREF").Count > 0)
            {
                data.FCCRef = doc.GetElementsByTagName("FCCREF").Item(0).InnerText;
            }
            if (doc.GetElementsByTagName("WCODE").Count > 0)
            {
                data.Status = doc.GetElementsByTagName("WCODE").Item(0).InnerText;
                WriteLog("Status: " + data.Status);
            }
            if (doc.GetElementsByTagName("EDESC").Count > 0)
            {
                data.Status = data.Status + " " + doc.GetElementsByTagName("EDESC").Item(0).InnerText;
                WriteLog("Status: " + data.Status);
            }

            XmlNodeList Warning = doc.GetElementsByTagName("WARNING");
            foreach (XmlNode node in Warning)
            {
                XmlElement warningElement = (XmlElement)node;
                data.Status = data.Status + " " + warningElement.GetElementsByTagName("WDESC")[0].InnerText;
            }
            data.CBSResponse = data.Status;

            return data;
        }

        public string OutwardTransaction(string Branch, string XREF, string HV, string RemAccount, string BenefAccount, string CheckSlNo, string CCY, string Amount,
                          string TxnDate, string ValDate, string InstDate, string RoutingNo, string TxnBrn, string RemBranch, string InstType, string ReasonCode, string RejectReason, string Envelop, string ProductCode,string UtilityBill)
        {
            string xmlstring = "";

            if (Envelop == "OCE")
                if (UtilityBill.ToUpper() == "FALSE")
                {
                    xmlstring = OutwardTransactionString(Branch, XREF, HV, RemAccount, BenefAccount, CheckSlNo, CCY, Amount,
                                TxnDate, ValDate, InstDate, RoutingNo, TxnBrn, RemBranch, InstType, ReasonCode, RejectReason, Envelop, ProductCode);
                }
                else
                {
                    xmlstring = OutwardUtilityBillTransactionString(Branch, XREF, HV, RemAccount, BenefAccount, CheckSlNo, CCY, Amount,
                                TxnDate, ValDate, InstDate, RoutingNo, TxnBrn, RemBranch, InstType, ReasonCode, RejectReason, Envelop, ProductCode);
                }
            return ExecuteCBSWebService(xmlstring);
        }
        private string OutwardUtilityBillTransactionString(string Branch, string XREF, string HV, string RemAccount, string BenefAccount, string CheckSlNo, string CCY, string Amount,
                            string TxnDate, string ValDate, string InstDate, string RoutingNo, string TxnBrn, string RemBranch, string InstType, string ReasonCode, string RejectReason, string Envelop, string ProductCode)
        {
            return "<S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                    "<S:Body>" +
                        "<CREATETRANSACTION_IOPK_REQ xmlns=\"" + AppVariable.OCREATETRANSACTION_IOPK_REQ + "\">" +
                        "<FCUBS_HEADER>" +
                            "<SOURCE>" + AppVariable.OSOURCE + "</SOURCE>" +
                            "<UBSCOMP>" + AppVariable.OUBSCOMP + "</UBSCOMP>" +
                            "<MSGID></MSGID>" +
                            "<CORRELID></CORRELID>" +
                            "<USERID>" + AppVariable.OUSERID + "</USERID>" +
                            "<BRANCH>" + Branch + "</BRANCH>" +
                            "<MODULEID>" + AppVariable.OMODULEID + "</MODULEID>" +
                            "<SERVICE>" + AppVariable.OSERVICE + "</SERVICE>" +
                            "<OPERATION>CreateTransaction</OPERATION>" +
                            "<SOURCE_OPERATION>CreateTransaction</SOURCE_OPERATION>" +
                            "<SOURCE_USERID></SOURCE_USERID>" +
                            "<DESTINATION></DESTINATION>" +
                            "<MULTITRIPID></MULTITRIPID>" +
                            "<FUNCTIONID></FUNCTIONID>" +
                            "<ACTION></ACTION>" +
                            "<PASSWORD></PASSWORD>" +
                            "<ADDL>" +
                                "<PARAM>" +
                                    "<NAME></NAME>" +
                                    "<VALUE></VALUE>" +
                                "</PARAM>" +
                            "</ADDL>" +
                        "</FCUBS_HEADER>" +
                        "<FCUBS_BODY>" +
                            "<Transaction-Details>" +
                                "<XREF>" + XREF + "</XREF>" +
                                "<SCODE>CGFSFS</SCODE>" +
                                "<CONSOLIDATED>N</CONSOLIDATED>" +
                                "<Clearing-Details>" +
                                    "<PRODUCT>" + ProductCode + "</PRODUCT>" +
                                    "<REMACCOUNT>" + RemAccount + "</REMACCOUNT>" +
                                    "<INSTRNO>" + CheckSlNo + "</INSTRNO>" +
                                    "<INSTRCCY>" + CCY + "</INSTRCCY>" +
                                    "<INSTRAMT>" + Amount + "</INSTRAMT>" +
                                    "<INSTRTYPE>CHQ</INSTRTYPE>" +
                                    "<INSTRDATE>" + InstDate + "</INSTRDATE>" +
                                    "<TXNDATE>" + TxnDate + "</TXNDATE>" +
                                    "<VALDATE>" + ValDate + "</VALDATE>" +
                                    "<ROUTINGNO>" + RoutingNo + "</ROUTINGNO>" +
                                    "<SERIALNO>1</SERIALNO>" +
                                    "<ENTRYNO>1</ENTRYNO>" +
                                    "<TXNBRN>" + TxnBrn + "</TXNBRN>" +
                                    "<BENACCOUNT>" + BenefAccount + "</BENACCOUNT>" +
                                    "<TXNCCY>" + CCY + "</TXNCCY>" +
                                    "<ACCCY>" + CCY + "</ACCCY>" +
                                    "<ACCCYAMT>" + Amount + "</ACCCYAMT>" +
                                    "<DIRECTION>O</DIRECTION>" +
                                    "<REMBRANCH>" + RemBranch + "</REMBRANCH>" +
                                "</Clearing-Details>" +
                            "</Transaction-Details>" +
                        "</FCUBS_BODY>" +
                    "</CREATETRANSACTION_IOPK_REQ>" +
                "</S:Body>" +
            "</S:Envelope>";
        }

        private string OutwardTransactionString(string Branch, string XREF, string HV, string RemAccount, string BenefAccount, string CheckSlNo, string CCY, string Amount,
                            string TxnDate, string ValDate, string InstDate, string RoutingNo, string TxnBrn, string RemBranch, string InstType, string ReasonCode, string RejectReason, string Envelop, string ProductCode)
        {
            return "<S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                    "<S:Body>" +
                        "<CREATETRANSACTION_IOPK_REQ xmlns=\"" + AppVariable.OCREATETRANSACTION_IOPK_REQ + "\">" +
                        "<FCUBS_HEADER>" +
                            "<SOURCE>" + AppVariable.OSOURCE + "</SOURCE>" +
                            "<UBSCOMP>" + AppVariable.OUBSCOMP + "</UBSCOMP>" +
                            "<MSGID></MSGID>" +
                            "<CORRELID></CORRELID>" +
                            "<USERID>" + AppVariable.OUSERID + "</USERID>" +
                            "<BRANCH>" + Branch + "</BRANCH>" +
                            "<MODULEID>" + AppVariable.OMODULEID + "</MODULEID>" +
                            "<SERVICE>" + AppVariable.OSERVICE + "</SERVICE>" +
                            "<OPERATION>CreateTransaction</OPERATION>" +
                            "<SOURCE_OPERATION>CreateTransaction</SOURCE_OPERATION>" +
                            "<SOURCE_USERID></SOURCE_USERID>" +
                            "<DESTINATION></DESTINATION>" +
                            "<MULTITRIPID></MULTITRIPID>" +
                            "<FUNCTIONID></FUNCTIONID>" +
                            "<ACTION></ACTION>" +
                            "<PASSWORD></PASSWORD>" +
                            "<ADDL>" +
                                "<PARAM>" +
                                    "<NAME></NAME>" +
                                    "<VALUE></VALUE>" +
                                "</PARAM>" +
                            "</ADDL>" +
                        "</FCUBS_HEADER>" +
                        "<FCUBS_BODY>" +
                            "<Transaction-Details>" +
                                "<XREF>" + XREF + "</XREF>" +
                                "<SCODE>CGFSFS</SCODE>" +
                                "<CONSOLIDATED>N</CONSOLIDATED>" +
                                "<Clearing-Details>" +
                                    "<PRODUCT>" + ProductCode + "</PRODUCT>" +
                                    "<REMACCOUNT>" + RemAccount + "</REMACCOUNT>" +
                                    "<INSTRNO>" + CheckSlNo + "</INSTRNO>" +
                                    "<INSTRCCY>" + CCY + "</INSTRCCY>" +
                                    "<INSTRAMT>" + Amount + "</INSTRAMT>" +
                                    "<INSTRTYPE>CHQ</INSTRTYPE>" +
                                    "<INSTRDATE>" + InstDate + "</INSTRDATE>" +
                                    "<TXNDATE>" + TxnDate + "</TXNDATE>" +
                                    "<VALDATE>" + ValDate + "</VALDATE>" +
                                    "<ROUTINGNO>" + RoutingNo + "</ROUTINGNO>" +
                                    "<SERIALNO>1</SERIALNO>" +
                                    "<ENTRYNO>1</ENTRYNO>" +
                                    "<TXNBRN>" + TxnBrn + "</TXNBRN>" +
                                    "<BENACCOUNT>" + BenefAccount + "</BENACCOUNT>" +
                                    "<TXNCCY>" + CCY + "</TXNCCY>" +
                                    "<ACCCY>" + CCY + "</ACCCY>" +
                                    "<ACCCYAMT>" + Amount + "</ACCCYAMT>" +
                                    "<DIRECTION>O</DIRECTION>" +
                                    "<REMBRANCH>" + RemBranch + "</REMBRANCH>" +
                                "</Clearing-Details>" +
                            "</Transaction-Details>" +
                        "</FCUBS_BODY>" +
                    "</CREATETRANSACTION_IOPK_REQ>" +
                "</S:Body>" +
            "</S:Envelope>";
        }

        public string ExecuteCBSWebService(string xmlstring)
        {
            string resultxml = "";

            string CBSURI = AppVariable.CBSURI + "/FCUBSCGService/FCUBSCGService?WSDL";

            WebRequest req = null;
            WebResponse rsp = null;
            try
            {

                req = (HttpWebRequest)WebRequest.Create(CBSURI);
                req.Method = "POST";
                req.ContentType = "text/xml";
                req.Credentials = CredentialCache.DefaultNetworkCredentials;

                StreamWriter writer = new StreamWriter(req.GetRequestStream());
                writer.WriteLine(xmlstring);
                writer.Close();

                rsp = (HttpWebResponse)req.GetResponse();

                StreamReader sr = new StreamReader(rsp.GetResponseStream());
                resultxml = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex)
            {
                resultxml = ex.Message;
            }

            //WriteLog("Output string:\n" + resultxml);

            return resultxml;

        }


    }
}

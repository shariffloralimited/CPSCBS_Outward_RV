using System;
using System.Configuration;

namespace CPSCBS
{
    public class AppVariable
    {
        public static string ServerLogin = "server=" + ConfigurationManager.AppSettings["DBServer"] + ";database=ACH;uid=floraweb;pwd=platinumfloor967";
        public static string LogPath = ConfigurationManager.AppSettings["LogPath"];


        public static string SIGUSER = ConfigurationManager.AppSettings["SIGUSER"];
        public static string SIGPASSWORD = ConfigurationManager.AppSettings["SIGPASSWORD"];
        public static string PASSWORD = ConfigurationManager.AppSettings["password"];
        public static string ACTINQSERV = ConfigurationManager.AppSettings["ACTINQSERV"];
        public static string CBSURI = ConfigurationManager.AppSettings["CBSURI"];
        public static string IROUTINGNO = ConfigurationManager.AppSettings["IROUTINGNO"];
        public static string ACCSOURCE = ConfigurationManager.AppSettings["ACCSOURCE"];


        //Inward 
        public static string ICREATETRANSACTION_IOPK_REQ = ConfigurationManager.AppSettings["ICREATETRANSACTION_IOPK_REQ"];
        public static string ISOURCE = ConfigurationManager.AppSettings["ISOURCE"];
        public static string IUBSCOMP = ConfigurationManager.AppSettings["IUBSCOMP"];
        public static string IUSERID = ConfigurationManager.AppSettings["IUSERID"];
        public static string IMODULEID = ConfigurationManager.AppSettings["IMODULEID"];
        public static string ISERVICE = ConfigurationManager.AppSettings["ISERVICE"];
        public static string IOPERATION = ConfigurationManager.AppSettings["IOPERATION"];
        public static string ISOURCE_OPERATION = ConfigurationManager.AppSettings["ISOURCE_OPERATION"];
        public static string IBRANCH = ConfigurationManager.AppSettings["IBRANCH"];

        //Outward 
        public static string OCREATETRANSACTION_IOPK_REQ = ConfigurationManager.AppSettings["ICREATETRANSACTION_IOPK_REQ"];
        public static string OSOURCE = ConfigurationManager.AppSettings["OSOURCE"];
        public static string OUBSCOMP = ConfigurationManager.AppSettings["OUBSCOMP"];
        public static string OUSERID = ConfigurationManager.AppSettings["OUSERID"];
        public static string OMODULEID = ConfigurationManager.AppSettings["OMODULEID"];
        public static string OSERVICE = ConfigurationManager.AppSettings["OSERVICE"];
        public static string OOPERATION = ConfigurationManager.AppSettings["OOPERATION"];
        public static string OSOURCE_OPERATION = ConfigurationManager.AppSettings["OSOURCE_OPERATION"];
        public static string OBRANCH = ConfigurationManager.AppSettings["OBRANCH"];
        public static string OROUTINGNO = ConfigurationManager.AppSettings["OROUTINGNO"];

        //ORE 
        public static string ORESOURCE = ConfigurationManager.AppSettings["ORESOURCE"];
        public static string OREUBSCOMP = ConfigurationManager.AppSettings["OREUBSCOMP"];
        public static string OREUSERID = ConfigurationManager.AppSettings["OREUSERID"];
        public static string OREBRANCH = ConfigurationManager.AppSettings["OREBRANCH"];
        public static string OREMODULEID = ConfigurationManager.AppSettings["OREMODULEID"];
        public static string ORESERVICE = ConfigurationManager.AppSettings["ORESERVICE"];
        public static string OREOPERATION = ConfigurationManager.AppSettings["OREOPERATION"];
        public static string ORESOURCE_OPERATION = ConfigurationManager.AppSettings["ORESOURCE_OPERATION"];
        public static string OREROUTINGNO = ConfigurationManager.AppSettings["OREROUTINGNO"];

        //IRE 
        public static string IRESOURCE = ConfigurationManager.AppSettings["IRESOURCE"];
        public static string IREUBSCOMP = ConfigurationManager.AppSettings["IREUBSCOMP"];
        public static string IREUSERID = ConfigurationManager.AppSettings["IREUSERID"];
        public static string IREBRANCH = ConfigurationManager.AppSettings["IREBRANCH"];
        public static string IREMODULEID = ConfigurationManager.AppSettings["IREMODULEID"];
        public static string IRESERVICE = ConfigurationManager.AppSettings["IRESERVICE"];
        public static string IREOPERATION = ConfigurationManager.AppSettings["IREOPERATION"];
        public static string IRESOURCE_OPERATION = ConfigurationManager.AppSettings["IRESOURCE_OPERATION"];
        public static string IREROUTINGNO = ConfigurationManager.AppSettings["IREROUTINGNO"];
        //Common
        public static string SOURCE = ConfigurationManager.AppSettings["SOURCE"];
        public static string UBSCOMP = ConfigurationManager.AppSettings["UBSCOMP"];
        public static string USERID = ConfigurationManager.AppSettings["USERID"];
        public static string BRANCH = ConfigurationManager.AppSettings["BRANCH"];
        public static string MODULEID = ConfigurationManager.AppSettings["MODULEID"];
        public static string SERVICE = ConfigurationManager.AppSettings["SERVICE"];
        public static string OPERATION = ConfigurationManager.AppSettings["OPERATION"];
        public static string SOURCE_OPERATION = ConfigurationManager.AppSettings["SOURCE_OPERATION"];


        public static string RT_SERVICE_DR_CREATETRANSACTION_FSFS_REQ = ConfigurationManager.AppSettings["RT_SERVICE_DR_CREATETRANSACTION_FSFS_REQ"];
        public static string RT_SERVICE_DR_SOURCE = ConfigurationManager.AppSettings["RT_SERVICE_DR_SOURCE"];
        public static string RT_SERVICE_DR_UBSCOMP = ConfigurationManager.AppSettings["RT_SERVICE_DR_UBSCOMP"];
        public static string RT_SERVICE_DR_USERID = ConfigurationManager.AppSettings["RT_SERVICE_DR_USERID"];
        public static string RT_SERVICE_DR_BRANCH = ConfigurationManager.AppSettings["RT_SERVICE_DR_BRANCH"];
        public static string RT_SERVICE_DR_MODULEID = ConfigurationManager.AppSettings["RT_SERVICE_DR_MODULEID"];
        public static string RT_SERVICE_DR_SERVICE = ConfigurationManager.AppSettings["RT_SERVICE_DR_SERVICE"];
        public static string RT_SERVICE_DR_OPERATION = ConfigurationManager.AppSettings["RT_SERVICE_DR_OPERATION"];
        public static string RT_SERVICE_DR_SOURCE_OPERATION = ConfigurationManager.AppSettings["RT_SERVICE_DR_SOURCE_OPERATION"];

        public static string RT_SERVICE_CR_CREATETRANSACTION_FSFS_REQ = ConfigurationManager.AppSettings["RT_SERVICE_CR_CREATETRANSACTION_FSFS_REQ"];
        public static string RT_SERVICE_CR_SOURCE = ConfigurationManager.AppSettings["RT_SERVICE_CR_SOURCE"];
        public static string RT_SERVICE_CR_UBSCOMP = ConfigurationManager.AppSettings["RT_SERVICE_CR_UBSCOMP"];
        public static string RT_SERVICE_CR_USERID = ConfigurationManager.AppSettings["RT_SERVICE_CR_USERID"];
        public static string RT_SERVICE_CR_BRANCH = ConfigurationManager.AppSettings["RT_SERVICE_CR_BRANCH"];
        public static string RT_SERVICE_CR_MODULEID = ConfigurationManager.AppSettings["RT_SERVICE_CR_MODULEID"];
        public static string RT_SERVICE_CR_SERVICE = ConfigurationManager.AppSettings["RT_SERVICE_CR_SERVICE"];
        public static string RT_SERVICE_CR_OPERATION = ConfigurationManager.AppSettings["RT_SERVICE_CR_OPERATION"];
        public static string RT_SERVICE_CR_SOURCE_OPERATION = ConfigurationManager.AppSettings["RT_SERVICE_CR_SOURCE_OPERATION"];
    }
}

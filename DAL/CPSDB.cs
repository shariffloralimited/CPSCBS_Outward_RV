using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPSCBS
{
    class CPSDB
    {

        //public DataTable GetTransactionList()
        //{
        //    SqlConnection myConnection = new SqlConnection(AppVariable.ServerLogin);
        //    SqlDataAdapter myCommand = new SqlDataAdapter("CBS_GetTransactionList", myConnection);
        //    myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

        //    myConnection.Open();
        //    DataTable dt = new DataTable();
        //    myCommand.Fill(dt);

        //    myConnection.Close();
        //    myCommand.Dispose();
        //    myConnection.Dispose();

        //    return dt;
        //}
        
        //public DataTable GetOutwardList()
        //{
        //    SqlConnection myConnection = new SqlConnection(AppVariable.ServerLogin);
        //    SqlDataAdapter myCommand = new SqlDataAdapter("CBS_GetListOutward", myConnection);
        //    myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

        //    myConnection.Open();
        //    DataTable dt = new DataTable();
        //    myCommand.Fill(dt);

        //    myConnection.Close();
        //    myCommand.Dispose();
        //    myConnection.Dispose();

        //    return dt;
        //}


        //public DataTable GetInwardList()
        //{
        //    SqlConnection myConnection = new SqlConnection(AppVariable.ServerLogin);
        //    SqlDataAdapter myCommand = new SqlDataAdapter("CBS_GetListInward", myConnection);
        //    myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

        //    myConnection.Open();
        //    DataTable dt = new DataTable();
        //    myCommand.Fill(dt);

        //    myConnection.Close();
        //    myCommand.Dispose();
        //    myConnection.Dispose();

        //    return dt;
        //}
        public void UpdateOutwardStatus(CBSData data)
        {
            if (data.Status.Length > 95)
                data.Status=data.Status.Substring(0, 95);
            if (data.CBSResponse.Length > 95)
                data.CBSResponse = data.CBSResponse.Substring(0, 95);

            SqlConnection myConnection = new SqlConnection(AppVariable.ServerLogin);
            SqlCommand myCommand = new SqlCommand("CBS_UpdateOutwardStatus", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterFloraID = new SqlParameter("@XREF", SqlDbType.VarChar, 50);
            parameterFloraID.Value = data.XREF;
            myCommand.Parameters.Add(parameterFloraID);

            SqlParameter parameterStatusCode = new SqlParameter("@Status", SqlDbType.VarChar, 100);
            parameterStatusCode.Value = data.Status;
            myCommand.Parameters.Add(parameterStatusCode);

            SqlParameter parameterErr_OD = new SqlParameter("@CBSResponse", SqlDbType.VarChar, 100);
            parameterErr_OD.Value = data.CBSResponse;
            myCommand.Parameters.Add(parameterErr_OD);

            SqlParameter parameterT24TxnID = new SqlParameter("@FCCRef", SqlDbType.VarChar, 50);
            parameterT24TxnID.Value = data.FCCRef;
            myCommand.Parameters.Add(parameterT24TxnID);

            SqlParameter parameterMsgID = new SqlParameter("@MsgID", SqlDbType.VarChar, 50);
            parameterMsgID.Value = data.MsgID;
            myCommand.Parameters.Add(parameterMsgID);

            SqlParameter parameterEnvelop = new SqlParameter("@Envelop", SqlDbType.VarChar, 4);
            parameterEnvelop.Value = data.Envelop;
            myCommand.Parameters.Add(parameterEnvelop);

            myConnection.Open();
            myCommand.ExecuteNonQuery();
            
            myConnection.Close();
            myCommand.Dispose();
            myConnection.Dispose();
        }

        public DataTable GetOutwardTransactionList(string HV)
        {
            SqlConnection myConnection = new SqlConnection(AppVariable.ServerLogin);
            SqlDataAdapter myCommand = new SqlDataAdapter("CBS_GetOutwardTransactionList", myConnection);
            myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterEnvelop = new SqlParameter("@HV", SqlDbType.VarChar,5);
            parameterEnvelop.Value = HV;
            myCommand.SelectCommand.Parameters.Add(parameterEnvelop);

            myConnection.Open();
            DataTable dt = new DataTable();
            myCommand.Fill(dt);

            myConnection.Close();
            myCommand.Dispose();
            myConnection.Dispose();

            return dt;
        }

        public void UpdateInwardStatus(CBSData data)
        {
            SqlConnection myConnection = new SqlConnection(AppVariable.ServerLogin);
            SqlCommand myCommand = new SqlCommand("CBS_UpdateInwardStatus", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterXREF = new SqlParameter("@XREF", SqlDbType.VarChar, 50);
            parameterXREF.Value = data.XREF;
            myCommand.Parameters.Add(parameterXREF);

            SqlParameter parameterCBSResponse = new SqlParameter("@CBSResponse", SqlDbType.VarChar, 500);
            parameterCBSResponse.Value = data.CBSResponse;
            myCommand.Parameters.Add(parameterCBSResponse);

            SqlParameter parameterMsgID = new SqlParameter("@MsgID", SqlDbType.VarChar, 50);
            parameterMsgID.Value = data.MsgID;
            myCommand.Parameters.Add(parameterMsgID);

            SqlParameter parameterFCCRef = new SqlParameter("@FCCRef", SqlDbType.VarChar, 50);
            parameterFCCRef.Value = data.FCCRef;
            myCommand.Parameters.Add(parameterFCCRef);

            SqlParameter parameterStatus = new SqlParameter("@Status", SqlDbType.VarChar, 500);
            parameterStatus.Value = data.Status;
            myCommand.Parameters.Add(parameterStatus);

            SqlParameter parameterEnvelop = new SqlParameter("@Envelop", SqlDbType.VarChar, 4);
            parameterEnvelop.Value = data.Envelop;
            myCommand.Parameters.Add(parameterEnvelop);

            myConnection.Open();
            myCommand.ExecuteNonQuery();

            myConnection.Close();
            myCommand.Dispose();
            myConnection.Dispose();
        }

        public void InsertTransactionReffOfAOCE(string XREF, string QueryTransaction_Status, string FCCREF, string INSTRNO, string REMACCOUNT, string ROUTINGNO,string checkId)
        {
            if (QueryTransaction_Status.Length > 500)
                QueryTransaction_Status = QueryTransaction_Status.Substring(0, 500);
            SqlConnection con = new SqlConnection(AppVariable.ServerLogin);
            con.Open();
            SqlCommand cmd = new SqlCommand("CBS_InsertTransactionReffOfAOCE", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@XREF", XREF);
            cmd.Parameters.AddWithValue("@QueryTransaction_Status", QueryTransaction_Status);
            cmd.Parameters.AddWithValue("@FCCREF", FCCREF);
            cmd.Parameters.AddWithValue("@INSTRNO", INSTRNO);
            cmd.Parameters.AddWithValue("@REMACCOUNT", REMACCOUNT);
            cmd.Parameters.AddWithValue("@ROUTINGNO", ROUTINGNO);
            cmd.Parameters.AddWithValue("@CheckID", new Guid(checkId));
            cmd.CommandTimeout = 600;
            cmd.ExecuteNonQuery();
        }

        public void InsertTransactionReffOfAIRE(string XREF, string QueryTransaction_Status, string FCCREF, string INSTRNO, string REMACCOUNT, string ROUTINGNO, string checkId)
        {
            if (QueryTransaction_Status.Length > 150)
                QueryTransaction_Status = QueryTransaction_Status.Substring(0, 150);
            SqlConnection con = new SqlConnection(AppVariable.ServerLogin);
            con.Open();
            SqlCommand cmd = new SqlCommand("CBS_InsertTransactionReffOfAIRE", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@XREF", XREF);
            cmd.Parameters.AddWithValue("@QueryTransaction_Status", QueryTransaction_Status);
            cmd.Parameters.AddWithValue("@FCCREF", FCCREF);
            cmd.Parameters.AddWithValue("@INSTRNO", INSTRNO);
            cmd.Parameters.AddWithValue("@REMACCOUNT", REMACCOUNT);
            cmd.Parameters.AddWithValue("@ROUTINGNO", ROUTINGNO);
            cmd.Parameters.AddWithValue("@CheckID", new Guid(checkId));
            cmd.CommandTimeout = 600;
            cmd.ExecuteNonQuery();
        }

        public void UpdateQueryTransaction_StatusOfAOCE(string XREF, string QueryTransaction_Status)
        {
            if (QueryTransaction_Status.Length > 150)
                QueryTransaction_Status = QueryTransaction_Status.Substring(0, 150);
            SqlConnection con = new SqlConnection(AppVariable.ServerLogin);
            con.Open();
            SqlCommand cmd = new SqlCommand("CBS_UpdateQueryTransaction_StatusOfAOCE", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@XREF", XREF);
            cmd.Parameters.AddWithValue("@QueryTransaction_Status", QueryTransaction_Status);
            cmd.CommandTimeout = 600;
            cmd.ExecuteNonQuery();
        }

        public void UpdateQueryTransaction_StatusOfAIRE(string XREF, string QueryTransaction_Status, string FCCREF, string INSTRNO, string REMACCOUNT, string ROUTINGNO)
        {
            if (QueryTransaction_Status.Length > 150)
                QueryTransaction_Status = QueryTransaction_Status.Substring(0, 150);

            SqlConnection con = new SqlConnection(AppVariable.ServerLogin);
            con.Open();
            SqlCommand cmd = new SqlCommand("CBS_UpdateQueryTransaction_StatusOfAIRE", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@XREF", XREF);
            cmd.Parameters.AddWithValue("@QueryTransaction_Status", QueryTransaction_Status);
            cmd.Parameters.AddWithValue("@FCCREF", FCCREF);
            cmd.Parameters.AddWithValue("@INSTRNO", INSTRNO);
            cmd.Parameters.AddWithValue("@REMACCOUNT", REMACCOUNT);
            cmd.Parameters.AddWithValue("@ROUTINGNO", ROUTINGNO);
            cmd.CommandTimeout = 600;
            cmd.ExecuteNonQuery();
        }

        internal string GetProductCode(string cCY, string ClearingType, string Session, bool NoCharge)
        {
            SqlConnection myConnection = new SqlConnection(AppVariable.ServerLogin);
            SqlDataAdapter myCommand = new SqlDataAdapter("CBS_GetProductCode", myConnection);
            myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;
            myCommand.SelectCommand.CommandTimeout = 360;

            SqlParameter parametercCY = new SqlParameter("@CCY", SqlDbType.VarChar, 3);
            parametercCY.Value = cCY;
            myCommand.SelectCommand.Parameters.Add(parametercCY);

            SqlParameter parameterClearingType = new SqlParameter("@ClearingType", SqlDbType.VarChar, 2);
            parameterClearingType.Value = ClearingType;
            myCommand.SelectCommand.Parameters.Add(parameterClearingType);

            SqlParameter parameterSession = new SqlParameter("@Session", SqlDbType.VarChar, 2);
            parameterSession.Value = Session;
            myCommand.SelectCommand.Parameters.Add(parameterSession);

            SqlParameter parameterNoCharge = new SqlParameter("@NoCharge", SqlDbType.Bit);
            parameterNoCharge.Value = NoCharge;
            myCommand.SelectCommand.Parameters.Add(parameterNoCharge);

            myConnection.Open();
            DataTable dt = new DataTable();
            myCommand.Fill(dt);

            myConnection.Close();
            myCommand.Dispose();
            myConnection.Dispose();

            if (dt.Rows.Count > 0)
                return dt.Rows[0]["ProductCode"].ToString();
            else return "";

        }

        public void UpdateQueryTransaction_StatusOfAORE(string XREF, string QueryTransaction_Status, string FCCREF, string INSTRNO, string REMACCOUNT, string ROUTINGNO)
        {
            if(QueryTransaction_Status.Length>150)
            {
                QueryTransaction_Status = QueryTransaction_Status.Substring(0, 150);
            }
            SqlConnection con = new SqlConnection(AppVariable.ServerLogin);
            con.Open();
            SqlCommand cmd = new SqlCommand("CBS_UpdateQueryTransaction_StatusOfOIRE", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@XREF", XREF);
            cmd.Parameters.AddWithValue("@QueryTransaction_Status", QueryTransaction_Status);
            cmd.Parameters.AddWithValue("@FCCREF", FCCREF);
            cmd.Parameters.AddWithValue("@INSTRNO", INSTRNO);
            cmd.Parameters.AddWithValue("@REMACCOUNT", REMACCOUNT);
            cmd.Parameters.AddWithValue("@ROUTINGNO", ROUTINGNO);
            cmd.CommandTimeout = 600;
            cmd.ExecuteNonQuery();
        }
    }
}

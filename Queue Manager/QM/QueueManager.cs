using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Queue_Manager.QM
{
    class QueueManager
    {
        public static string Token = "";
        public static void settokenQM()
        {
            try
            { 
            var client = new RestClient(Form1.__URL + "token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("password", Form1.__password);
            request.AddParameter("grant_type", "password");
            IRestResponse response = client.Execute(request);

     
                
                if (response.StatusCode == 0)
                throw new Exception("Não foi possível contactar o servidor remoto!");

            JavaScriptSerializer js = new JavaScriptSerializer();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var ret = js.Deserialize<Models.token>(response.Content);
                    //ret.access_token;
                    //ret.token_type;
                    Token = "Bearer " + ret.access_token;
                //return ret.access_token;
            }
            else
                throw new Exception(response.Content);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public static void sendsingleXML(string content, out string status, out string link, out string msg)
        {
            string _status = "";
            string _link = "";
            string _msg = "";
            try
            {
                var client = new RestClient(Form1.__URL + "/api/xmlimporter/single");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Log", "True");
                request.AddHeader("Authorization", Token);
                request.AddHeader("Content-Type", "text/plain");
                request.AddParameter("text/plain", content, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                if (response.StatusCode == 0)
                    throw new Exception("Não foi possível contactar o servidor remoto!");

                JavaScriptSerializer js = new JavaScriptSerializer();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var ret = js.Deserialize<Models.single>(response.Content);
                    //ret.access_token;
                    //ret.token_type;
                    _status = ret.Sucess;
                    _link = ret.Description.Code;
                    _msg = ret.Description.Description;

                    //return ret.access_token;
                }
                else
                    throw new Exception(response.Content);
            }
            catch
            {

            }

            status = _status;
            link = _link;
            msg = _msg;

        }
        public static void getstatus(string URL, out string status, out string link, out string msg)
        {
            string _status = "";
            string _link = "";
            string _msg = "";
            try
            {
                var client = new RestClient(Form1.__URL + "/api/xmlimporter/" + URL);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Log", "True");
                request.AddHeader("Authorization", Token);
                request.AddParameter("text/plain", "", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);


                if (response.StatusCode == 0)
                    throw new Exception("Não foi possível contactar o servidor remoto!");

                JavaScriptSerializer js = new JavaScriptSerializer();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var ret = js.Deserialize<Models.CheckStatus>(response.Content);
                    //ret.access_token;
                    //ret.token_type;
                    _status = ret.Items[0].Status;
                    _link = ret.Items[0].ResultCode;
                    _msg = ret.Items[0].ResultDescription;
                    //return ret.access_token;
                }
                else
                    throw new Exception(response.Content);
            }
            catch
            {

            }

            status = _status;
            link = _link;
            msg = _msg;

        }

        public static void SendBatchXML(string content, out string status, out string link, out string msg)
        {
            string _status = "";
            string _link = "";
            string _msg = "";
            try
            {
                var client = new RestClient(Form1.__URL + "/api/xmlimporter/batch");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                //request.AddHeader("Log", "True");
                request.AddHeader("Authorization", Token);
                request.AddHeader("Accept", "application/json");
                request.AddParameter("application/json", content, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == 0)
                    throw new Exception("Não foi possível contactar o servidor remoto!");

                JavaScriptSerializer js = new JavaScriptSerializer();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var ret = js.Deserialize<Models.single>(response.Content);
                    //ret.access_token;
                    //ret.token_type;
                    _status = ret.Sucess;
                    _link = ret.Description.Code;
                    _msg = ret.Description.Description;

                    //return ret.access_token;
                }
                else
                    throw new Exception(response.Content);
            }
            catch
            {

            }

            status = _status;
            link = _link;
            msg = _msg;

        }
    }
}

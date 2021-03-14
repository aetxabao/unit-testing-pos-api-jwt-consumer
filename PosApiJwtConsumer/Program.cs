using System;
using System.Collections.Generic;
using System.Threading;
using PosApiJwtConsumer.Items;
using RestSharp; //dotnet add package RestSharp

namespace PosApiJwtConsumer
{
    public class Consumer
    {
        public const string BASEURL = "https://localhost:5001/api";

        private const string USERNAME = "test";
        private const string PASSWORD = "M3l0c0t0n3s!";
        // private const string USERNAME = "other";
        // private const string PASSWORD = "T0m4t3s!";
        // private const string USERNAME = "wrong";
        // private const string PASSWORD = "xxxxxxxx";

        public static void Main(string[] args)
        {
            LoginRequest loginReq;
            LoginResponse loginResp;
            List<Message> list;
            Message message, reply = new Message();
            string msg;

            // USUARIO
            Console.WriteLine("\nUsername:\n" + USERNAME);

            // TOKEN
            loginReq = new LoginRequest { 
                UserName = USERNAME, 
                Password = PASSWORD 
            };
            loginResp = PostLogin(loginReq);
            Console.WriteLine("\nTOKEN:\n" + loginResp.Token);

            // RECIBIR LISTADO
            Console.WriteLine("\nLIST:");
            list = GetMessages(loginResp.Token);
            foreach (var x in list)
            {
                Console.WriteLine(x.MsgHeader());
            }

            // RESPONDER CADA MENSAJE RECIBIDO Y ELIMINARLO
            Console.WriteLine("\nREPLY:");
            foreach (var x in list)
            {
                // RECIBIR
                message = GetMessage(loginResp.Token, x.MessageId);
                // ENVIAR RESPUESTA
                msg = "RECIBIDO: " + message.MsgBody.Msg;
                reply = new Message
                {
                    To = message.From,
                    From = USERNAME,
                    MsgBody = new MsgBody { 
                        Msg = msg, 
                        Stamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") 
                    }
                };
                reply = PostMessage(loginResp.Token, reply);
                Console.WriteLine(reply);
                // BORRAR
                DeleteMessage(loginResp.Token, x.MessageId);
            }

            // ACTUALIZAR ÚLTIMO MENSAJE ENVIADO
            Console.WriteLine("\nACTUALIZADO:");
            if (list.Count > 0){
                msg = reply.MsgBody.Msg.Replace("RECIBIDO: ","Recibido: ");
                reply.MsgBody.Msg = msg;
                message = PutMessage(loginResp.Token, reply);
                Console.WriteLine(message);
            }

            // RECIBIR LISTADO VACIO
            Console.WriteLine("\nLIST:");
            list = GetMessages(loginResp.Token);
            foreach (var x in list)
            {
                Console.WriteLine(x.MsgHeader());
            }
            Console.WriteLine();
        }

        public static LoginResponse PostLogin(LoginRequest login)
        {
            //TODO: PostLogin
            var client = new RestClient(BASEURL);
            var request = new RestRequest("Users/login", Method.POST); //esto sale en el swagger
            //request.AddParameter("data", data);
            request.AddJsonBody(login.ToJson());
            var response = client.Execute(request);
            // Console.WriteLine("Content: " + response.Content);
            // Console.WriteLine("Status Code: " + response.StatusCode);//NotFound|Created|BadRequest
            if (response.StatusCode.ToString().Contains("BadRequest"))
            {
                return new LoginResponse { Token = response.Content };
            }
            return LoginResponse.FromJson(response.Content);          
        }

        public static List<Message> GetMessages(string token)
        {
            //TODO: GetMessages
            var client = new RestClient(BASEURL);
            var request = new RestRequest("Messages", Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = client.Execute(request);
            // Console.WriteLine("\nGetMessages:");
            // Console.WriteLine(response.Content);
            if (!response.Content.Contains("Invalid user") || response.Content.Trim().Length != 0)
            {
                return Message.ListFromJson(response.Content);
            }
            return null;
        }

        public static Message GetMessage(string token, int messageId)
        {
            //TODO: GetMessage
            var client = new RestClient(BASEURL);
            var request = new RestRequest($"/Messages/{messageId}", Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = client.Execute(request);
            // Console.WriteLine("Content: " + response.Content);
            // Console.WriteLine("Status Code: " + response.StatusCode);//NotFound|OK
            if (!response.Content.Contains("Invalid user") || response.Content.Trim().Length != 0)
            {
                return Message.FromJson(response.Content);
            }
            return null;
        }

        public static Message PostMessage(string token, Message message)
        {
            //TODO: PostMessage
            var client = new RestClient(BASEURL);
            var request = new RestRequest("Messages", Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);

            request.AddJsonBody(message.ToJson());
            var response = client.Execute(request);
            //Console.WriteLine(response.Content);
            //Console.WriteLine(response.StatusCode);//NotFound|Created
            if (!response.Content.Contains("Invalid user") || response.Content.Trim().Length != 0)
            {
                return Message.FromJson(response.Content);
            }
            return null;
        }

        public static Message PutMessage(string token, Message message)
        {
            //TODO: PutMessage
            var client = new RestClient(BASEURL);
            var request = new RestRequest($"Messages/{message.MessageId}", Method.PUT);
            request.AddJsonBody(message.ToJson());
            var response = client.Execute(request);
            if (!response.Content.Contains("Invalid user") || response.Content.Trim().Length != 0)
            {
                return Message.FromJson(response.Content);
            }
            return null;
        }

        public static void DeleteMessage(string token, int id)
        {
            //TODO: DeleteMessage
            var client = new RestClient(BASEURL);
            var request = new RestRequest($"Messages/{id}", Method.DELETE);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = client.Execute(request);
        }
    }
}

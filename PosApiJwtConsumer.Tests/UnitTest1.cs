using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Xunit;
using Xunit.Extensions.Ordering;
using PosApiJwtConsumer.Items;
//dotnet add package Xunit.Extensions.Ordering
[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]

namespace PosApiJwtConsumer.Tests
{
    // https://xunit.net/docs/shared-context
    // You can use the class fixture feature of xUnit.net to share a single object instance among all tests in a test class.    
    public class MyTestFixture
    {
        public string POSTMSG0, POSTMSG1, POSTMSG2, POSTMSG = "TOPSECRET";
        public string PUTMSG = "XXXXXXXXX";
        private readonly Random random = new Random();
        public string tokenTest;
        public string usernameTest;
        public string tokenOther;
        public string usernameOther;
        public string tokenWrong;
        public string usernameWrong;
        // Se insertarán 3 mensajes con los identificadores id+0, id+1, id+2
        public int id;

        // Utiliza PostLogin y PostMessage!!! Inicialmente se obtienen los tokens y se insertan 2 mensajes. 
        public MyTestFixture()
        {
            int n = random.Next(10, 97);
            POSTMSG0 = POSTMSG + n;
            PUTMSG += n++;
            POSTMSG1 = POSTMSG + n++;
            POSTMSG2 = POSTMSG + n;
            LoginRequest loginReq = new LoginRequest { UserName = "test", Password = "M3l0c0t0n3s!" };
            var loginResp = Consumer.PostLogin(loginReq);
            tokenTest = loginResp.Token;
            usernameTest = loginResp.UserName;
            loginReq = new LoginRequest { UserName = "other", Password = "T0m4t3s!" };
            loginResp = Consumer.PostLogin(loginReq);
            tokenOther = loginResp.Token;
            usernameOther = loginResp.UserName;
            loginReq = new LoginRequest { UserName = "wrong", Password = "xxxxxxxx" };
            loginResp = Consumer.PostLogin(loginReq);
            tokenWrong = loginResp.Token;
            usernameWrong = loginResp.UserName;
            Message m = new Message
            {
                To = "other",
                From = "test",
                MsgBody = new MsgBody { Msg = POSTMSG0, Stamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") }
            };
            m = Consumer.PostMessage(tokenTest, m);
            id = m.MessageId;
            m = new Message
            {
                To = "other",
                From = "test",
                MsgBody = new MsgBody { Msg = POSTMSG1, Stamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") }
            };
            m = Consumer.PostMessage(tokenTest, m);
        }
    }

    // https://opensource.com/article/20/3/failed-authentication-attempts-tdd
    // To force your tests to run in a certain sequence that you define (instead of running in an unpredictable order), 
    // you need to extend the xUnit toolkit with the NuGet Xunit.Extensions.Ordering package.
    public class UnitTest1 : IClassFixture<MyTestFixture>
    {
        MyTestFixture fixture;
        public UnitTest1(MyTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact, Order(1)]
        public void Test1_PostLogin()
        {
            var exp = @"([_=A-Za-z0-9\-]+)\.([_=A-Za-z0-9\-]+)\.([_=A-Za-z0-9\-]+)";
            Assert.True(fixture.usernameTest == "test", "El nombre del usuario es test.");
            Match match = Regex.Match(fixture.tokenTest, exp, RegexOptions.IgnoreCase);
            Assert.True(match.Success, "El token de test está separado por dos puntos.");
            Assert.True(fixture.usernameOther == "other", "El nombre del usuario es other.");
            match = Regex.Match(fixture.tokenOther, exp, RegexOptions.IgnoreCase);
            Assert.True(match.Success, "El token de other está separado por dos puntos.");
            Assert.True(fixture.usernameWrong == "wrong", "El nombre del usuario es wrong.");
            match = Regex.Match(fixture.tokenWrong, exp, RegexOptions.IgnoreCase);
            Assert.True(!match.Success, "El token de wrong NO está separado por dos puntos.");
            Assert.True(fixture.tokenWrong == "Invalid credentials", "Wrong utiliza credenciales invalidas.");
        }

        [Fact, Order(2)]
        public void Test2_PostMessage()
        {
            Message m = new Message
            {
                To = "other",
                From = "test",
                MsgBody = new MsgBody { Msg = fixture.POSTMSG2, Stamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") }
            };
            m = Consumer.PostMessage(fixture.tokenTest, m);
            Assert.True(m.MessageId > 0, "Id mayor que cero.");
            Assert.True(m.To == "other" && m.From == "test", "Mensaje de other para test.");
            Assert.True(m.MsgBody.Msg == fixture.POSTMSG2, $"Mensaje {fixture.POSTMSG2} enviado.");
        }

        [Fact, Order(3)]
        public void Test3_PutMessage()
        {
            Message m = new Message
            {
                MessageId = fixture.id,
                To = "other",
                From = "test",
                MsgBody = new MsgBody { Msg = fixture.PUTMSG, Stamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") }
            };
            m = Consumer.PutMessage(fixture.tokenTest, m);
            Assert.True(m.MessageId == fixture.id, $"El id es {fixture.id}.");
            Assert.True(m.To == "other" && m.From == "test", "El To y From están bien.");
            Assert.True(m.MsgBody.Msg == fixture.PUTMSG, $"El mensaje contiene {fixture.PUTMSG}.");
        }

        [Fact, Order(4)]
        public void Test4_GetMessage()
        {
            Message m = Consumer.GetMessage(fixture.tokenOther, fixture.id);
            Assert.True(m.MessageId == fixture.id, $"El id {fixture.id} coincide");
            Assert.True(m.To == "other" && m.From == "test", "El To y From coincide");
            Assert.True(m.MsgBody.Msg == fixture.PUTMSG, $"El mensaje {fixture.PUTMSG} coincide con {m.MsgBody.Msg}.");
        }

        [Fact, Order(5)]
        public void Test5_DeleteMessage()
        {
            int i = fixture.id + 1;
            Consumer.DeleteMessage(fixture.tokenOther, i);
            Message m = Consumer.GetMessage(fixture.tokenOther, i);
            Assert.True(m.MessageId == 0, $"El id es 0");
            Assert.True(m.To == "other" && m.From == "server", "Es un mensaje del servidor.");
            Assert.True(m.MsgBody.Msg == $"Message {i} was not found", $"El mensaje {i} no existe.");
        }

        [Fact, Order(6)]
        public void Test6_GetMessages()
        {
            bool b0 = false, b1 = false, b2 = false;
            Thread.Sleep(2000);
            List<Message> list = Consumer.GetMessages(fixture.tokenOther);
            Assert.True(list.Count >= 2, "Al menos hay dos mensajes");
            foreach (var m in list)
            {
                if (m.MessageId == fixture.id) b0 = true;
                if (m.MessageId == fixture.id + 1) b1 = true;
                if (m.MessageId == fixture.id + 2) b2 = true;
            }
            Assert.True(b0 && !b1 && b2, $"Están los dos mensajes correctos. {fixture.id}:{b0}, {fixture.id + 1}:{b1}, {fixture.id + 2}:{b2}");
            Consumer.DeleteMessage(fixture.tokenOther, fixture.id);
            Consumer.DeleteMessage(fixture.tokenOther, fixture.id + 2);
            b0 = b1 = b2 = false;
            list = Consumer.GetMessages(fixture.tokenOther);
            foreach (var m in list)
            {
                if (m.MessageId == fixture.id) b0 = true;
                if (m.MessageId == fixture.id + 1) b1 = true;
                if (m.MessageId == fixture.id + 2) b2 = true;
            }
            Assert.True(!b0 && !b1 && !b2, "No quedan mensajes.");
        }

        [Fact, Order(7)]
        public void Test7_GetMessages()
        {
            LoginRequest loginReq = new LoginRequest { UserName = "aetxabao", Password = "P4t4t4s!" };
            LoginResponse loginResp = Consumer.PostLogin(loginReq);
            string token = loginResp.Token;
            List<Message> list = Consumer.GetMessages(token);
            foreach (var m in list)
            {
                Consumer.DeleteMessage(token, m.MessageId);
            }
            list = Consumer.GetMessages(token);
            Assert.True(list.Count == 0, $"Tiene {list.Count} mensajes.");
        }

        [Fact, Order(8)]
        public void Test8_GetMessages()
        {
            LoginRequest loginReq = new LoginRequest
            {
                UserName = "aetxabao",
                Password = "xxxxxxxx"
            };
            LoginResponse loginResp = Consumer.PostLogin(loginReq);
            string token = loginResp.Token;
            List<Message> list = Consumer.GetMessages(token);
            list = Consumer.GetMessages(token);
            Assert.True(list.Count == 0, $"Accede a {list.Count} mensajes.");
        }
    }
}

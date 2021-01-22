using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queue_Manager.QM
{
    class Models
    {
        //Token
        public class token
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
        }



        //Single
        public class single
        {
            public string Sucess { get; set; }
            public _Description Description { get; set; }
        }
        public class _Description
        {
            public string Code { get; set; }
            public string Description { get; set; }
            public string Route { get; set; }
        }

        //CheckStatus
        public class CheckStatus
        {
            public string Id { get; set; }
            public string CreatedDate { get; set; }
            public string UseCase { get; set; }
            public string Done { get; set; }

            public List<_Items> Items { get; set; }



        }
        public class _Items
        {
            public string ResultCode { get; set; }
            public string ResultDescription { get; set; }
            public string Keys { get; set; }
            public string Status { get; set; }
        }

    }
}

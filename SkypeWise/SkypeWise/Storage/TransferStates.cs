using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkypeWise.Storage
{
    static public class TransferStates
    {
        static List<Transfer> TransferList=new List<Transfer>();

        public static string AddTransfer(string transferFrom, string transferTo, double ammount, string currency="$")
        {
            Transfer tr = new Transfer(transferFrom, transferTo, ammount);

            TransferList.Add(tr);

            return "Created new transfer with ID: " + tr.UID + "\nDo you want to approve it?";
        }
    }
}
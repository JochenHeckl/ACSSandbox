 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Protocol
{
    public class LoginRequest
    {
        public string UserId { get; set; }
        public string LoginToken { get; set; }
    }
}
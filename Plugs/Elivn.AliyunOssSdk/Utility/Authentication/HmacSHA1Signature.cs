﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Elivn.AliyunOssSdk.Utility.Authentication
{
    public class HmacSHA1Signature: ServiceSignature
    {
        private readonly Encoding _encoding = Encoding.UTF8;

        public override string SignatureMethod
        {
            get { return "HmacSHA1"; }
        }

        public override string SignatureVersion
        {
            get { return "1"; }
        }

        protected override string ComputeSignatureCore(string key, string data)
        {
            Debug.Assert(!string.IsNullOrEmpty(data));

            //using (var algorithm = KeyedHashAlgorithm.Create(
            //    SignatureMethod.ToString().ToUpperInvariant()))
            //{
            //    algorithm.Key = _encoding.GetBytes(key.ToCharArray());
            //    return Convert.ToBase64String(
            //        algorithm.ComputeHash(_encoding.GetBytes(data.ToCharArray())));
            //}

            var bytes = _encoding.GetBytes(key);
            Trace.WriteLine("Key:" + key + " Data:" + data);
            Trace.WriteLine(bytes);

            using (var algorithm = new HMACSHA1(_encoding.GetBytes(key)))
            {
                //algorithm.Key = _encoding.GetBytes(key.ToCharArray());
                var result = Convert.ToBase64String(
                    algorithm.ComputeHash(_encoding.GetBytes(data.ToCharArray())));
                Trace.WriteLine("Result:" + result);
                return result;
            }
        }
    }
}

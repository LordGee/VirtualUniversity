using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class UtilityScript {

    public static string EncryptPassword(string pw) {

        byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        byte[] iv  = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

        SymmetricAlgorithm algorithm    = DES.Create();
        ICryptoTransform   transform    = algorithm.CreateEncryptor(key, iv);
        byte[]             inputbuffer  = Encoding.Unicode.GetBytes(pw);
        byte[]             outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
        string             result       = Convert.ToBase64String(outputBuffer);

        return result;
    }

}

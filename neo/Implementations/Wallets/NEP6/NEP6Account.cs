﻿using Neo.IO.Json;
using Neo.Wallets;
using System;

namespace Neo.Implementations.Wallets.NEP6
{
    internal class NEP6Account : WalletAccount
    {
        private readonly NEP6Wallet wallet;
        private readonly string nep2key;
        private KeyPair key;
        public JObject Extra;

        public bool Decrypted => nep2key == null || key != null;
        public override bool HasKey => nep2key != null;

        public NEP6Account(NEP6Wallet wallet, UInt160 scriptHash, string nep2key = null)
            : base(scriptHash)
        {
            this.wallet = wallet;
            this.nep2key = nep2key;
        }

        public NEP6Account(NEP6Wallet wallet, UInt160 scriptHash, KeyPair key, string password)
            : this(wallet, scriptHash, key.Export(password, wallet.Scrypt.N, wallet.Scrypt.R, wallet.Scrypt.P))
        {
            this.key = key;
        }

        /// <summary>
        /// 初始化账号 Add Code
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="scriptHash"></param>
        /// <param name="key"></param>
        public NEP6Account(NEP6Wallet wallet, UInt160 scriptHash, KeyPair key)
            : base(scriptHash)
        {
            this.wallet = wallet;
            this.key = key;
        }

        public static NEP6Account FromJson(JObject json, NEP6Wallet wallet)
        {
            return new NEP6Account(wallet, Wallet.ToScriptHash(json["address"].AsString()), json["key"]?.AsString())
            {
                Label = json["label"]?.AsString(),
                IsDefault = json["isDefault"].AsBoolean(),
                Lock = json["lock"].AsBoolean(),
                Contract = NEP6Contract.FromJson(json["contract"]),
                Extra = json["extra"]
            };
        }

        /// <summary>
        /// 通过账号json对象转为账号对象
        /// 只允许使用json像中的地址和公钥
        /// AddCode
        /// </summary>
        /// <param name="json">json对象</param>
        /// <param name="wallet">钱包</param>
        /// <returns></returns>
        public static NEP6Account fromJson(JObject json, NEP6Wallet wallet)
        {
            return new NEP6Account(wallet, Wallet.ToScriptHash(json["address"].AsString()), new KeyPair(json["public"].AsString()))
            {
                Label = json["label"]?.AsString(),
                IsDefault = json["isDefault"].AsBoolean(),
                Lock = json["lock"].AsBoolean(),
                Contract = NEP6Contract.FromJson(json["contract"]),
                Extra = json["extra"]
            };
        }

        public override KeyPair GetKey()
        {
            if (nep2key == null) return null;
            if (key == null)
            {
                key = wallet.DecryptKey(nep2key);
            }
            return key;
        }

        public KeyPair GetKey(string password)
        {
            if (nep2key == null) return null;
            if (key == null)
            {
                key = new KeyPair(Wallet.GetPrivateKeyFromNEP2(nep2key, password, wallet.Scrypt.N, wallet.Scrypt.R, wallet.Scrypt.P));
            }
            return key;
        }

        public JObject ToJson()
        {
            JObject account = new JObject();
            account["address"] = Wallet.ToAddress(ScriptHash);
            account["label"] = Label;
            account["isDefault"] = IsDefault;
            account["lock"] = Lock;
            account["key"] = nep2key;
            account["contract"] = ((NEP6Contract)Contract)?.ToJson();
            account["extra"] = Extra;
            return account;
        }

        /// <summary>
        /// 将账号转成JSON对象
        /// </summary>
        /// <returns></returns>
        public JObject toJson()
        {
            JObject account = new JObject();
            account["address"] = Wallet.ToAddress(ScriptHash);
            account["label"] = Label;
            account["isDefault"] = IsDefault;
            account["lock"] = Lock;
            //account["private"] = key.PrivateKey.ToString();// 私钥   测试用  需要删除
            account["public"] = key.PublicKey.ToString(); // 公钥
            //account["key"] = nep2key; //nep2key 私钥和密码加密后的字符串 测试用  需要删除
            account["contract"] = ((NEP6Contract)Contract)?.ToJson();
            account["extra"] = Extra;
            return account;
        }

        /// <summary>
        /// 对外输出账号信息 private key and address
        /// AddCode
        /// </summary>
        /// <returns>private key and address</returns>
        public override JObject OutputJson()
        {
            JObject account = new JObject();
            account["address"] = Wallet.ToAddress(ScriptHash);
            account["private"] = GetPrivateKey();
            return account;
        }

        public bool VerifyPassword(string password)
        {
            try
            {
                Wallet.GetPrivateKeyFromNEP2(nep2key, password, wallet.Scrypt.N, wallet.Scrypt.R, wallet.Scrypt.P);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        /// <summary>
        /// 获取 16 进制私钥
        /// Add Code 
        /// </summary>
        /// <returns></returns>
        public override string GetPrivateKey()
        {
            return key.PrivateKey.ToHexString();
        }

        /// <summary>
        /// 打印账号信息
        /// AddCode
        /// </summary>
        public override void Print()
        {
            Console.WriteLine($"            prikey: {GetPrivateKey()} ");
            Console.WriteLine($"            wifkey: {GetWIFKey()} ");
            Console.WriteLine($"           nep2key: {nep2key}");
            Console.WriteLine($"            pubkey: {(key?.PublicKey.EncodePoint(true).ToHexString())}");
            Console.WriteLine($"           address: {Wallet.ToAddress(ScriptHash)}");
            Console.WriteLine($"   contract script: {((NEP6Contract)Contract)?.Script.ToHexString()}");
        }

        /// <summary>
        /// 获取 WIF 私钥 (对外使用的)
        /// Add Code
        /// </summary>
        /// <returns>WIF 私钥</returns>
        public override string GetWIFKey()
        {
            return key.Export();
        }

    }
}

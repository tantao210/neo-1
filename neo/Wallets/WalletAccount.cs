using Neo.IO.Json;
using Neo.SmartContract;

namespace Neo.Wallets
{
    public abstract class WalletAccount
    {
        public readonly UInt160 ScriptHash;
        public string Label;
        public bool IsDefault;
        public bool Lock;
        public Contract Contract;

        public string Address => Wallet.ToAddress(ScriptHash);
        public abstract bool HasKey { get; }
        public bool WatchOnly => Contract == null;

        public abstract KeyPair GetKey();

        /// <summary>
        /// 获取私钥
        /// </summary>
        /// <returns></returns>
        public abstract string GetPrivateKey();

        /// <summary>
        /// 获取WifKey
        /// </summary>
        /// <returns></returns>
        public abstract string GetWIFKey();

        /// <summary>
        /// 打印账号信息
        /// </summary>
        public abstract void Print();

        /// <summary>
        /// 对外输出账号信息 private key and address
        /// AddCode
        /// </summary>
        /// <returns>private key and address</returns>
        public abstract JObject OutputJson();

        protected WalletAccount(UInt160 scriptHash)
        {
            this.ScriptHash = scriptHash;
        }
    }
}

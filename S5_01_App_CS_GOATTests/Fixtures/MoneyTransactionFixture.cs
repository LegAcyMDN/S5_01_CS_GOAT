using System;
using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class MoneyTransactionFixture
    {
        public static MoneyTransaction GetMoneyTransaction()
        {
            return new MoneyTransaction
            {
                TransactionId = 1,
                UserId = 2,
                TransactionDate = DateTime.Now,
                WalletValue = 50.00,
                PaymentMethodId = 1
            };
        }

        public static MoneyTransaction GetOtherUserMoneyTransaction()
        {
            return new MoneyTransaction
            {
                TransactionId = 2,
                UserId = 1,
                TransactionDate = DateTime.Now,
                WalletValue = 100.00,
                PaymentMethodId = 1
            };
        }

        public static List<MoneyTransaction> GetMoneyTransactions()
        {
            return new List<MoneyTransaction>
            {
                new MoneyTransaction
                {
                    TransactionId = 1,
                    UserId = 2,
                    TransactionDate = DateTime.Now,
                    WalletValue = 50.00,
                    PaymentMethodId = 1
                },
                new MoneyTransaction
                {
                    TransactionId = 3,
                    UserId = 2,
                    TransactionDate = DateTime.Now,
                    WalletValue = 75.00,
                    PaymentMethodId = 2
                }
            };
        }

        public static List<MoneyTransaction> GetAllMoneyTransactions()
        {
            return new List<MoneyTransaction>
            {
                new MoneyTransaction
                {
                    TransactionId = 1,
                    UserId = 2,
                    TransactionDate = DateTime.Now,
                    WalletValue = 50.00,
                    PaymentMethodId = 1
                },
                new MoneyTransaction
                {
                    TransactionId = 2,
                    UserId = 1,
                    TransactionDate = DateTime.Now,
                    WalletValue = 100.00,
                    PaymentMethodId = 1
                },
                new MoneyTransaction
                {
                    TransactionId = 3,
                    UserId = 2,
                    TransactionDate = DateTime.Now,
                    WalletValue = 75.00,
                    PaymentMethodId = 2
                }
            };
        }
    }
}

using System;
using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.DTO;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class TransactionFixture
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

        // PaymentMethod fixtures
        public static PaymentMethod GetPaymentMethod()
        {
            return new PaymentMethod
            {
                PaymentMethodId = 1,
                PaymentMethodName = "Credit Card",
                FromWallet = false,
                ToWallet = true
            };
        }

        public static List<PaymentMethod> GetPaymentMethods()
        {
            return new List<PaymentMethod>
            {
                new PaymentMethod
                {
                    PaymentMethodId = 1,
                    PaymentMethodName = "Credit Card",
                    FromWallet = false,
                    ToWallet = true
                },
                new PaymentMethod
                {
                    PaymentMethodId = 2,
                    PaymentMethodName = "Wallet Withdrawal",
                    FromWallet = true,
                    ToWallet = false
                }
            };
        }

        // ItemTransaction fixtures
        public static ItemTransaction GetItemTransaction()
        {
            return new ItemTransaction
            {
                TransactionId = 1,
                UserId = 2,
                InventoryItemId = 1,
                TransactionDate = DateTime.Now.AddDays(-5),
                WalletValue = 15.50,
                CancelledOn = null
            };
        }

        public static ItemTransaction GetOtherUserItemTransaction()
        {
            return new ItemTransaction
            {
                TransactionId = 2,
                UserId = 1,
                InventoryItemId = 2,
                TransactionDate = DateTime.Now.AddDays(-3),
                WalletValue = 25.00,
                CancelledOn = null
            };
        }

        public static List<ItemTransaction> GetItemTransactions()
        {
            return new List<ItemTransaction>
            {
                new ItemTransaction
                {
                    TransactionId = 1,
                    UserId = 2,
                    InventoryItemId = 1,
                    TransactionDate = DateTime.Now.AddDays(-5),
                    WalletValue = 15.50,
                    CancelledOn = null
                },
                new ItemTransaction
                {
                    TransactionId = 3,
                    UserId = 2,
                    InventoryItemId = 3,
                    TransactionDate = DateTime.Now.AddDays(-2),
                    WalletValue = 45.00,
                    CancelledOn = null
                }
            };
        }

        public static ItemTransactionDTO GetItemTransactionDTO()
        {
            return new ItemTransactionDTO
            {
                InventoryItemId = 1,
                TransactionDate = DateTime.Now.AddDays(-5),
                WalletValue = 15.50,
                CancelledOn = null
            };
        }

        public static ItemTransactionDetailDTO GetItemTransactionDetailDTO()
        {
            return new ItemTransactionDetailDTO
            {
                InventoryItemId = 1,
                TransactionDate = DateTime.Now.AddDays(-5),
                WalletValue = 15.50,
                CancelledOn = null,
                WearName = "Field-Tested",
                SkinName = "Redline",
                ItemName = "AK-47",
                ItemTypeName = "Rifle",
                Uuid = "test-uuid-1",
                RarityColor = "#4B69FF"
            };
        }

        public static List<ItemTransactionDTO> GetItemTransactionDTOs()
        {
            return new List<ItemTransactionDTO>
            {
                new ItemTransactionDTO
                {
                    InventoryItemId = 1,
                    TransactionDate = DateTime.Now.AddDays(-5),
                    WalletValue = 15.50,
                    CancelledOn = null
                },
                new ItemTransactionDTO
                {
                    InventoryItemId = 3,
                    TransactionDate = DateTime.Now.AddDays(-2),
                    WalletValue = 45.00,
                    CancelledOn = null
                }
            };
        }

        // RandomTransaction fixtures
        public static RandomTransaction GetRandomTransaction()
        {
            return new RandomTransaction
            {
                TransactionId = 1,
                UserId = 2,
                TransactionDate = DateTime.Now.AddDays(-3),
                WalletValue = -10.00,
                CaseId = 1,
                FairRandomId = 1
            };
        }

        public static RandomTransaction GetOtherUserRandomTransaction()
        {
            return new RandomTransaction
            {
                TransactionId = 2,
                UserId = 1,
                TransactionDate = DateTime.Now.AddDays(-2),
                WalletValue = -15.00,
                CaseId = 2,
                FairRandomId = 2
            };
        }

        public static List<RandomTransaction> GetRandomTransactions()
        {
            return new List<RandomTransaction>
            {
                new RandomTransaction
                {
                    TransactionId = 1,
                    UserId = 2,
                    TransactionDate = DateTime.Now.AddDays(-3),
                    WalletValue = -10.00,
                    CaseId = 1,
                    FairRandomId = 1
                },
                new RandomTransaction
                {
                    TransactionId = 3,
                    UserId = 2,
                    TransactionDate = DateTime.Now.AddDays(-1),
                    WalletValue = -12.50,
                    CaseId = 1,
                    FairRandomId = 3
                }
            };
        }

        public static RandomTransactionDetailDTO GetRandomTransactionDetailDTO()
        {
            return new RandomTransactionDetailDTO
            {
                TransactionDate = DateTime.Now.AddDays(-3),
                WalletValue = -10.00
            };
        }

        public static List<RandomTransactionDTO> GetRandomTransactionDTOs()
        {
            return new List<RandomTransactionDTO>
            {
                new RandomTransactionDTO
                {
                    TransactionDate = DateTime.Now.AddDays(-3),
                    WalletValue = -10.00
                },
                new RandomTransactionDTO
                {
                    TransactionDate = DateTime.Now.AddDays(-1),
                    WalletValue = -12.50
                }
            };
        }

        // UpgradeResult fixtures
        public static UpgradeResult GetUpgradeResult()
        {
            return new UpgradeResult
            {
                InventoryItemId = 1,
                TransactionId = 1,
                FairRandomId = 1,
                FloatStart = 0.15f,
                FloatEnd = 0.18f,
                ProbIntact = 0.70,
                ProbDegrade = 0.25,
                PropDestroy = 0.05,
                DegradeFunction = "Linear"
            };
        }

        public static List<UpgradeResult> GetUpgradeResults()
        {
            return new List<UpgradeResult>
            {
                new UpgradeResult
                {
                    InventoryItemId = 1,
                    TransactionId = 1,
                    FairRandomId = 1,
                    FloatStart = 0.15f,
                    FloatEnd = 0.18f,
                    ProbIntact = 0.70,
                    ProbDegrade = 0.25,
                    PropDestroy = 0.05,
                    DegradeFunction = "Linear"
                },
                new UpgradeResult
                {
                    InventoryItemId = 1,
                    TransactionId = 2,
                    FairRandomId = 2,
                    FloatStart = 0.18f,
                    FloatEnd = 0.22f,
                    ProbIntact = 0.65,
                    ProbDegrade = 0.30,
                    PropDestroy = 0.05,
                    DegradeFunction = "Linear"
                }
            };
        }

        public static UpgradeResultDTO GetUpgradeResultDTO()
        {
            return new UpgradeResultDTO
            {
                FloatStart = 0.15f,
                FloatEnd = 0.18f,
                ProbIntact = 0.70,
                ProbDegrade = 0.25,
                PropDestroy = 0.05,
                DegradeFunction = "Linear"
            };
        }

        public static List<UpgradeResultDTO> GetUpgradeResultDTOs()
        {
            return new List<UpgradeResultDTO>
            {
                new UpgradeResultDTO
                {
                    FloatStart = 0.15f,
                    FloatEnd = 0.18f,
                    ProbIntact = 0.70,
                    ProbDegrade = 0.25,
                    PropDestroy = 0.05,
                    DegradeFunction = "Linear"
                },
                new UpgradeResultDTO
                {
                    FloatStart = 0.18f,
                    FloatEnd = 0.22f,
                    ProbIntact = 0.65,
                    ProbDegrade = 0.30,
                    PropDestroy = 0.05,
                    DegradeFunction = "Linear"
                }
            };
        }
    }
}

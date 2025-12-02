using System;
using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.DTO;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class ItemTransactionFixture
    {
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
    }
}

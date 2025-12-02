using System;
using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class PromoCodeFixture
    {
        public static PromoCode GetPromoCode()
        {
            return new PromoCode
            {
                PromoCodeId = 1,
                Code = "SUMMER2024",
                DiscountPercentage = 15,
                DiscountAmount = 10.00,
                ExpiryDate = DateTime.Now.AddDays(30),
                CaseId = 1,
                UserId = null
            };
        }

        public static PromoCode GetExpiredPromoCode()
        {
            return new PromoCode
            {
                PromoCodeId = 2,
                Code = "EXPIRED2023",
                DiscountPercentage = 20,
                DiscountAmount = 15.00,
                ExpiryDate = DateTime.Now.AddDays(-10),
                CaseId = null,
                UserId = 2
            };
        }

        public static PromoCode GetUserPromoCode()
        {
            return new PromoCode
            {
                PromoCodeId = 3,
                Code = "USERSPECIAL",
                DiscountPercentage = 10,
                DiscountAmount = 5.00,
                ExpiryDate = DateTime.Now.AddDays(15),
                CaseId = null,
                UserId = 2
            };
        }

        public static List<PromoCode> GetPromoCodes()
        {
            return new List<PromoCode>
            {
                new PromoCode
                {
                    PromoCodeId = 1,
                    Code = "SUMMER2024",
                    DiscountPercentage = 15,
                    DiscountAmount = 10.00,
                    ExpiryDate = DateTime.Now.AddDays(30),
                    CaseId = 1,
                    UserId = null
                },
                new PromoCode
                {
                    PromoCodeId = 2,
                    Code = "EXPIRED2023",
                    DiscountPercentage = 20,
                    DiscountAmount = 15.00,
                    ExpiryDate = DateTime.Now.AddDays(-10),
                    CaseId = null,
                    UserId = 2
                },
                new PromoCode
                {
                    PromoCodeId = 3,
                    Code = "USERSPECIAL",
                    DiscountPercentage = 10,
                    DiscountAmount = 5.00,
                    ExpiryDate = DateTime.Now.AddDays(15),
                    CaseId = null,
                    UserId = 2
                }
            };
        }

        public static PromoCode GetNewPromoCode()
        {
            return new PromoCode
            {
                Code = "RICK2024",
                DiscountPercentage = 25,
                DiscountAmount = 20.00,
                ExpiryDate = DateTime.Now.AddDays(60),
                CaseId = 2,
                UserId = null
            };
        }

        public static PromoCode GetUpdatedPromoCode()
        {
            return new PromoCode
            {
                PromoCodeId = 1,
                Code = "SUMMER2024UPDATED",
                DiscountPercentage = 20,
                DiscountAmount = 15.00,
                ExpiryDate = DateTime.Now.AddDays(45),
                CaseId = 1,
                UserId = null
            };
        }
    }
}

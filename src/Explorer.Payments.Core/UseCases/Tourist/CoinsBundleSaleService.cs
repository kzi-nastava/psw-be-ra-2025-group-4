using System;
using System.Collections.Generic;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Administration;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases.Administration
{
    public class CoinsBundleSaleService : ICoinsBundleSaleService
    {
        private readonly ICoinsBundleSaleRepository _saleRepository;
        private readonly ICoinsBundleRepository _bundleRepository;
        private readonly ICoinsBundlePurchaseRepository _purchaseRepository;
        private readonly IMapper _mapper;

        public CoinsBundleSaleService(
            ICoinsBundleSaleRepository saleRepository,
            ICoinsBundleRepository bundleRepository,
            ICoinsBundlePurchaseRepository purchaseRepository,
            IMapper mapper)
        {
            _saleRepository = saleRepository;
            _bundleRepository = bundleRepository;
            _purchaseRepository = purchaseRepository;
            _mapper = mapper;
        }

        public CoinsBundleSaleDto CreateSale(CoinsBundleSaleDto saleDto)
        {
            var bundle = _bundleRepository.Get(saleDto.CoinsBundleId);
            if (bundle == null)
                throw new KeyNotFoundException("Bundle not found.");

            var existingSale = _saleRepository.GetActiveSaleForBundle(saleDto.CoinsBundleId);
            if (existingSale != null && existingSale.IsCurrentlyActive())
                throw new InvalidOperationException("An active sale already exists for this bundle.");

            var sale = new CoinsBundleSale(
                saleDto.CoinsBundleId,
                saleDto.DiscountPercentage,
                saleDto.StartDate,
                saleDto.EndDate
            );

            var createdSale = _saleRepository.Create(sale);
            return _mapper.Map<CoinsBundleSaleDto>(createdSale);
        }

        public List<CoinsBundleSaleDto> GetAllSales()
        {
            var sales = _saleRepository.GetAll();
            return _mapper.Map<List<CoinsBundleSaleDto>>(sales);
        }

        public CoinsBundleSaleDto GetSale(int id)
        {
            var sale = _saleRepository.Get(id);
            if (sale == null)
                throw new KeyNotFoundException("Sale not found.");

            return _mapper.Map<CoinsBundleSaleDto>(sale);
        }

        public void DeactivateSale(int id)
        {
            var sale = _saleRepository.Get(id);
            if (sale == null)
                throw new KeyNotFoundException("Sale not found.");

            sale.Deactivate();
            _saleRepository.Update(sale);
        }

        public void DeleteSale(int id)
        {
            var sale = _saleRepository.Get(id);
            if (sale == null)
                throw new KeyNotFoundException("Sale not found.");

            _saleRepository.Delete(id);
        }

        public List<CoinsBundlePurchaseDto> GetAllPurchases()
        {
            var purchases = _purchaseRepository.GetAll();
            return _mapper.Map<List<CoinsBundlePurchaseDto>>(purchases);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.Contract.Interfaces;

namespace DATSYS.TradingModel.ServiceContracts.Interfaces
{
    internal interface IInternalTradingModel
    {
        void SetModel(ITradeModel model);


    }
}

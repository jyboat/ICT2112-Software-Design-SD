using ClearCare.Models;
using ClearCare.Gateways;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ClearCare.Controls
{
    public class SideEffectControl
    {
        private readonly SideEffectsMapper _sideEffectsMapper;

        public SideEffectControl(SideEffectsMapper sideEffectsMapper)
        {
            _sideEffectsMapper = sideEffectsMapper;
        }

        public async Task<List<SideEffectModel>> GetSideEffectsAsync()
        {
            return await _sideEffectsMapper.GetAllSideEffectsAsync();
        }
          public async Task AddSideEffectAsync(SideEffectModel sideEffect)
        {
            await _sideEffectsMapper.AddSideEffectAsync(sideEffect);
        }
    }
}
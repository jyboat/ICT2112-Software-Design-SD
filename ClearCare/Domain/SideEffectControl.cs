using ClearCare.Gateways;
using ClearCare.Models;
using ClearCare.Observers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Domain
{
    public class SideEffectControl
    {
        private readonly SideEffectsMapper _sideEffectsMapper;
        private readonly SideEffectSubject _sideEffectSubject;

        public SideEffectControl(SideEffectsMapper sideEffectsMapper, SideEffectSubject sideEffectSubject)
        {
            _sideEffectsMapper = sideEffectsMapper;
            _sideEffectSubject = sideEffectSubject;
        }

        public async Task<List<SideEffectModel>> GetSideEffectsAsync()
        {
            return await _sideEffectsMapper.GetAllSideEffectsAsync();
        }

        public async Task AddSideEffectAsync(SideEffectModel sideEffect)
        {
            await _sideEffectsMapper.AddSideEffectAsync(sideEffect);

            // Notify observers about the new side effect
            _sideEffectSubject.Notify(sideEffect, "Created");
        }
    }
}

using DrovoAPI.Classes;
using DrovoAPI.Interfaces;
using DrovoAPI.Models;
using DrovoAPI.NeuralNetwork.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace DrovoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEnginePool;
        private ISiteParser _siteParser;
        public StoreController(ISiteParser siteParser, PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool)
        {
            _siteParser = siteParser;
            _predictionEnginePool = predictionEnginePool;
        }
        // GET: api/<StoreController>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string query)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest(new { error = "Invalid param passed." });

            int categoryId = Predict(query);

            var result = await _siteParser.GetData(query, categoryId);

            return Ok(result);
        }

        private DataMap Map(List<IStoreMap> shopsData, int categoryId)
        {
            return new DataMap(shopsData, categoryId);
        }

        private int Predict(string query)
        {
            ModelInput sampleData = new ModelInput() { Col1 = query };
            ModelOutput prediction = _predictionEnginePool.Predict(sampleData);
            return (int)prediction.Prediction;
        }
    }
}

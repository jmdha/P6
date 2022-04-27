using DatabaseConnector.Connectors;
using QueryParser.Exceptions;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tools.Models;
using Tools.Models.JsonModels;
using Tools.Regex;

[assembly: InternalsVisibleTo("QueryParserTest")]

namespace QueryParser.QueryParsers
{
    public class JsonQueryParser : IQueryParser
    {
        public JsonQueryParser()
        {
        }

        public bool DoesQueryMatch(JsonQuery query)
        {
            return true;
        }

        public async Task<bool> DoesQueryMatchAsync(JsonQuery query)
        {
            return await new Task<bool>(() => DoesQueryMatch(query));
        }

        public ParserResult ParseQuery(JsonQuery query)
        {
            var newResult = new ParserResult();



            return newResult;
        }

        public async Task<ParserResult> ParseQueryAsync(JsonQuery query)
        {
            return await new Task<ParserResult>(() => ParseQuery(query));
        }
    }
}

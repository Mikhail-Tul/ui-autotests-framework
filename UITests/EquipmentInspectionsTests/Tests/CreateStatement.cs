using EquipmentInspectionsTests.Base;
using System.Text.Json;

namespace EquipmentInspectionsTests.Tests
{
    [TestClass]
    public sealed class CreateStatement : Auth
    {
        private async Task GetOrgUid()
        {
            var result = await APIContext.ExecuteLuaScriptAsync(
                        "Source/Lua/GetOrganizationUid.lua", new()
                        {
                            { "personName", Config.PersonName }
                        });
            foreach (var item in result.EnumerateArray())
            {
                OrgUid = item.ToString();
            }
        }

        private async Task GetSubstaionUid()
        {
            var result = await APIContext.ExecuteLuaScriptAsync(
                        "Source/Lua/GetSubstationUid.lua");
            foreach (var item in result.EnumerateArray())
            {
                SubstationUid = item.ToString();
            }
        }
        
        private async Task GetPersonUid()
        {
            var result = await APIContext.ExecuteLuaScriptAsync(
                        "Source/Lua/GetPersonUid.lua", new()
                        {
                            { "personName", Config.PersonName }
                        });
            foreach (var item in result.EnumerateArray())
            {
                PersonUid = item.ToString();
            }
        }

        [TestInitialize]
        public async Task Init()
        {
            await GetOrgUid();
            await GetSubstaionUid();
            await GetPersonUid();
            var jsonBuild = new JsonBuilder()
                .Add("name", Config.TextPattern)
                .Add("organisationUid", OrgUid)
                .AddArrayOfObjects("values", a => a
                    .AddArrayItem(i => i
                        .Add("name", Config.TextPattern)
                        .Add("orderIndex", 1)))
                .ToJson();
            var resMeas = await APIContext.PostAsync($"{Config.BaseUrl}{Config.DirectoriesApiPath}/measurement-values",
                new() { Data = jsonBuild });
            var jsonMeas = await resMeas.JsonAsync();
            jsonMeas!.Value.TryGetProperty("value", out var jsonEl);
            MeasurementValueUid = jsonEl.EnumerateObject().First().Value.ToString();
            jsonBuild = new JsonBuilder()
                .Add("name", Config.TextPattern)
                .Add("organisationUid", OrgUid)
                .AddArrayOfObjects("measurements", a => a
                    .AddArrayItem(i => i
                        .Add("name", Config.TextPattern)
                        .Add("isRequired", true)
                        .Add("valueType", "real")
                        .Add("warningLimitMin", "50")
                        .Add("warningLimitMax", "80")
                        .Add("emergencyLimitMin", "40")
                        .Add("emergencyLimitMax", "90")
                        .AddNull("directoryUid")
                        .Add("defaultValue", "60")))
                .ToJson();
            var resControl = await APIContext.PostAsync($"{Config.BaseUrl}{Config.DirectoriesApiPath}/control-objects",
                new() { Data = jsonBuild });
            var jsonControl = await resControl.JsonAsync();
            jsonControl!.Value.TryGetProperty("value", out jsonEl);
            ControlObjectUid = jsonEl.EnumerateObject().First().Value.ToString();
            jsonBuild = new JsonBuilder()
                .Add("name", Config.TextPattern)
                .AddNull("equipmentUid")
                .Add("organisationUid", OrgUid)
                .AddArrayOfObjects("points", a => a
                    .AddArrayItem(i => i
                        .Add("name", Config.TextPattern)
                        .Add("isRequired", true)
                        .Add("controlObjectUid", jsonEl.EnumerateObject().First().Value.ToString())))
                .ToJson();
            var resStatement = await APIContext.PostAsync($"{Config.BaseUrl}{Config.DirectoriesApiPath}/control-statement-types",
                new() { Data = jsonBuild });
            var jsonStatement = await resStatement.JsonAsync();
            jsonStatement!.Value.TryGetProperty("value", out jsonEl);
            StatementTypesUid = jsonEl.EnumerateObject().First().Value.ToString();
            jsonBuild = new JsonBuilder()
                .Add("name", Config.TextPattern)
                .Add("organisationUid", OrgUid)
                .AddArray("userUids", PersonUid)
                .AddEmptyArray("userGroups")
                .AddNull("statementReviewType")
                .AddNull("statementReviewFilter")
                .AddArray("canReadStatementTypes", jsonEl.EnumerateObject().First().Value.ToString())
                .AddArray("canWriteStatementTypes", jsonEl.EnumerateObject().First().Value.ToString())
                .Add("emailNotificationEnabled", false)
                .Add("statementReviewType", "accept")
                .AddObject("statementReviewFilter", i => i
                    .AddArray("statementTypeUids", jsonEl.EnumerateObject().First().Value.ToString()))
                .ToJson();
            var resUserRole = await APIContext.PostAsync($"{Config.BaseUrl}{Config.DirectoriesApiPath}/user-roles",
                new() { Data = jsonBuild });
            var jsonUserRole = await resUserRole.JsonAsync();
            jsonUserRole!.Value.TryGetProperty("value", out jsonEl);
            UserRoleUid = jsonEl.EnumerateObject().First().Value.ToString();
        }

        [TestMethod]
        public async Task CreateStatementAsync()
        {

        }

        [TestCleanup]
        public async Task CleanUp()
        {
            await APIContext.DeleteAsync($"{Config.BaseUrl}{Config.DirectoriesApiPath}/user-roles/{UserRoleUid}");
            await APIContext.DeleteAsync($"{Config.BaseUrl}{Config.DirectoriesApiPath}/control-statement-types/{StatementTypesUid}");
            await APIContext.DeleteAsync($"{Config.BaseUrl}{Config.DirectoriesApiPath}/control-objects/{ControlObjectUid}");
            await APIContext.DeleteAsync($"{Config.BaseUrl}{Config.DirectoriesApiPath}/measurement-values/{MeasurementValueUid}");
        }
    }
}

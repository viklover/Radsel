using Newtonsoft.Json.Linq;
using Radsel.Core.Model;
using Radsel.Core.Model.Battery;
using Radsel.Core.Model.Event;
using Radsel.Core.Model.Gate;
using Radsel.Core.Process;
using static Radsel.Core.Process.RadselClient;

namespace Radsel.Core.Test;
/// <summary>
///     Unit tests to <see cref="RadselClient"/>
/// </summary>
public class RadselClientTest : RadselTest {
    [TestCase("Keepalive", RadselSSEventType.KeepAlive)]
    [TestCase("NewEvents", RadselSSEventType.NewEvents)]
    [TestCase("Close", RadselSSEventType.Close)]
    public void ReadSSEventTest(string eventTypeRaw, RadselSSEventType expectedEventType) {
        var result = RadselClient.ReadSSEventType(eventTypeRaw);
        Assert.That(result, Is.EqualTo(expectedEventType));
    }
    [TestCase("InputPassive", RadselEventType.InputPassive)]
    [TestCase("InputActive", RadselEventType.InputActive)]
    [TestCase("PowerRecovery", RadselEventType.PowerRecovery)]
    [TestCase("PowerFault", RadselEventType.PowerFault)]
    [TestCase("BatteryLow1", RadselEventType.BatteryLow1)]
    [TestCase("BatteryLow2", RadselEventType.BatteryLow2)]
    [TestCase("BalanceLow", RadselEventType.BalanceLow)]
    [TestCase("TempLow", RadselEventType.TempLow)]
    [TestCase("TempNormal", RadselEventType.TempNormal)]
    [TestCase("TempHigh", RadselEventType.TempHigh)]
    [TestCase("CaseOpen", RadselEventType.CaseOpen)]
    [TestCase("Test", RadselEventType.Test)]
    [TestCase("Info", RadselEventType.Info)]
    [TestCase("Arm", RadselEventType.Arm)]
    [TestCase("Disarm", RadselEventType.Disarm)]
    [TestCase("Protect", RadselEventType.Protect)]
    [TestCase("ProfileApplied", RadselEventType.ProfileApplied)]
    [TestCase("DeviceOn", RadselEventType.DeviceOn)]
    [TestCase("DeviceRestart", RadselEventType.DeviceRestart)]
    [TestCase("FirmwareUpgrade", RadselEventType.FirmwareUpgrade)]
    [TestCase("ExtRuntimeError", RadselEventType.ExtRuntimeError)]
    [TestCase("NewGateRecords", RadselEventType.NewGateRecords)]
    public void ReadEventTypeTest(string eventTypeRaw, RadselEventType expectedEventType) {
        var result = ReadEventType(eventTypeRaw);
        Assert.That(result, Is.EqualTo(expectedEventType));
    }
    [TestCase("{\"ID\":35,\"Type\":\"Test\"}", 35, RadselEventType.Test)]
    public void ReadRawEventTest(string input, int expectedId, RadselEventType expectedEventType) {
        var inputJson = JToken.Parse(input);
        var result = ReadRawEvent(inputJson);
        Assert.That(result.Id.Id, Is.EqualTo(expectedId));
        Assert.That(result.Type, Is.EqualTo(expectedEventType));
    }
    [TestCase(typeof(RadselEventInputPassive), "{\"ID\":24,\"Type\":\"InputPassive\",\"Number\":1}")]
    [TestCase(typeof(RadselEventInputActive), "{\"ID\":25,\"Type\":\"InputActive\",\"Number\":16,\"Partitions\":[1,2,3,4]}")]
    [TestCase(typeof(RadselEventPowerRecovery), "{\"ID\":26,\"Type\":\"PowerRecovery\"}")]
    [TestCase(typeof(RadselEventPowerFault), "{\"ID\":26,\"Type\":\"PowerFault\"}")]
    [TestCase(typeof(RadselEventBatteryLow1), "{\"ID\":26,\"Type\":\"BatteryLow1\"}")]
    [TestCase(typeof(RadselEventBatteryLow2), "{\"ID\":26,\"Type\":\"BatteryLow2\"}")]
    [TestCase(typeof(RadselEventBalanceLow), "{\"ID\":26,\"Type\":\"BalanceLow\"}")]
    [TestCase(typeof(RadselEventTempLow), "{\"ID\":26,\"Type\":\"TempLow\"}")]
    [TestCase(typeof(RadselEventTempNormal), "{\"ID\":26,\"Type\":\"TempNormal\"}")]
    [TestCase(typeof(RadselEventTempHigh), "{\"ID\":26,\"Type\":\"TempHigh\"}")]
    [TestCase(typeof(RadselEventCaseOpen), "{\"ID\":26,\"Type\":\"CaseOpen\"}")]
    [TestCase(typeof(RadselEventTest), "{\"ID\":26,\"Type\":\"Test\"}")]
    [TestCase(typeof(RadselEventInfo), "{\"ID\":26,\"Type\":\"Info\"}")]
    [TestCase(typeof(RadselEventArm), "{\"ID\":44,\"Type\":\"Arm\",\"Partition\":4,\"Source\":{\"Type\":\"TouchMemory\",\"Key\":\"0001020304050607\",\"KeyName\":\"Vasya\"}}")]
    [TestCase(typeof(RadselEventDisarm), "{\"ID\":50,\"Type\":\"Disarm\",\"Partition\":2,\"Source\":{\"Type\":\"uGuardNet\",\"UserName\":\"Name\"}}")]
    [TestCase(typeof(RadselEventProtect), "{\"ID\":50,\"Type\":\"Protect\",\"Partition\":2,\"Source\":{\"Type\":\"uGuardNet\",\"UserName\":\"Name\"}}")]
    [TestCase(typeof(RadselEventProfileApplied), "{\"ID\":37,\"Type\":\"ProfileApplied\",\"Number\":1}")]
    [TestCase(typeof(RadselEventDeviceOn), "{\"ID\":38,\"Type\":\"DeviceOn\"}")]
    [TestCase(typeof(RadselEventDeviceRestart), "{\"ID\":38,\"Type\":\"DeviceRestart\"}")]
    [TestCase(typeof(RadselEventFirmwareUpgrade), "{\"ID\":38,\"Type\":\"FirmwareUpgrade\"}")]
    [TestCase(typeof(RadselEventExtRuntimeError), "{\"ID\":53,\"Type\":\"ExtRuntimeError\",\"ErrorCode\":1}")]
    [TestCase(typeof(RadselEventNewGateRecords), "{\"ID\":38,\"Type\":\"NewGateRecords\"}")]
    public void ReadEventTest(Type expectedEventType, string input) {
        var inputJson = JToken.Parse(input);
        var result = ReadEvent(inputJson);
        var isInstanceOfReadEvent = expectedEventType.IsInstanceOfType(result);
        Assert.That(isInstanceOfReadEvent, Is.True);
    }
    [TestCase("{\"Number\":16,\"Partitions\":[1,2,3,4]}", 16, 1, 2, 3, 4)]
    [TestCase("{\"Number\":16}", 16)]
    public void ReadEventInputPassiveTest(string input, int number, params int[] partitions) {
        var inputJson = JToken.Parse(input);
        var eventId = GenerateEventId();
        var result = ReadEventInputPassive(eventId, inputJson);
        Assert.That(result.Pin, Is.EqualTo(number));
        if (partitions.Length == 0) {
            Assert.That(result.Partitions, Is.Null);
        } else {
            Assert.That(result.Partitions, Is.EqualTo(partitions).AsCollection);
        }
    }
    [TestCase("{\"Number\":16,\"Partitions\":[1,2,3,4]}", 16, 1, 2, 3, 4)]
    [TestCase("{\"Number\":16}", 16)]
    public void ReadEventInputActiveTest(string input, int number, params int[] partitions) {
        var inputJson = JToken.Parse(input);
        var eventId = GenerateEventId();
        var result = ReadEventInputActive(eventId, inputJson);
        Assert.That(result.Pin, Is.EqualTo(number));
        if (partitions.Length == 0) {
            Assert.That(result.Partitions, Is.Null);
        } else {
            Assert.That(result.Partitions, Is.EqualTo(partitions).AsCollection);
        }
    }
    [TestCase("Button", RadselEventSourceType.Button)]
    [TestCase("Input", RadselEventSourceType.Input)]
    [TestCase("Scheduler", RadselEventSourceType.Scheduler)]
    [TestCase("Modbus", RadselEventSourceType.Modbus)]
    [TestCase("TouchMemory", RadselEventSourceType.TouchMemory)]
    [TestCase("DTMF", RadselEventSourceType.DTMF)]
    [TestCase("SMS", RadselEventSourceType.SMS)]
    [TestCase("CSD", RadselEventSourceType.CSD)]
    [TestCase("Call", RadselEventSourceType.Call)]
    [TestCase("GTNet", RadselEventSourceType.GTNet)]
    [TestCase("uGuardNet", RadselEventSourceType.UGuardNet)]
    [TestCase("Shell", RadselEventSourceType.Shell)]
    public void ReadEventSourceTypeTest(string input, RadselEventSourceType sourceType) {
        var result = ReadEventSourceType(input);
        Assert.That(result, Is.EqualTo(sourceType));
    }
    [TestCase(typeof(RadselEventSourceButton), "{\"Type\":\"Button\"}")]
    [TestCase(typeof(RadselEventSourceInput), "{\"Type\":\"Input\"}")]
    [TestCase(typeof(RadselEventSourceScheduler), "{\"Type\":\"Scheduler\"}")]
    [TestCase(typeof(RadselEventSourceModbus), "{\"Type\":\"Modbus\"}")]
    [TestCase(typeof(RadselEventSourceTouchMemory), "{\"Type\":\"TouchMemory\",\"Key\":\"0001020304050607\",\"KeyName\":\"Vasya\"}")]
    [TestCase(typeof(RadselEventSourceDTMF), "{\"Type\":\"DTMF\",\"Phone\":\"+71231234567\"}")]
    [TestCase(typeof(RadselEventSourceSMS), "{\"Type\":\"SMS\",\"Phone\":\"+71231234567\"}")]
    [TestCase(typeof(RadselEventSourceCSD), "{\"Type\":\"CSD\",\"Phone\":\"+71231234567\"}")]
    [TestCase(typeof(RadselEventSourceCall), "{\"Type\":\"Call\",\"Phone\":\"+71231234567\"}")]
    [TestCase(typeof(RadselEventSourceGTNet), "{\"Type\":\"GTNet\"}")]
    [TestCase(typeof(RadselEventSourceUGuardNet), "{\"Type\":\"uGuardNet\",\"UserName\":\"Name\"}")]
    [TestCase(typeof(RadselEventSourceShell), "{\"Type\":\"Shell\",\"UserName\":\"Name\"}")]
    public void ReadEventSourceTest(Type expectedEventType, string input) {
        var inputJson = JToken.Parse(input);
        var result = ReadEventSource(inputJson);
        var isInstanceOfReadEvent = expectedEventType.IsInstanceOfType(result);
        Assert.That(isInstanceOfReadEvent, Is.True);
    }
    [TestCase("Low2", RadselBatteryState.Low2)]
    [TestCase("Low1", RadselBatteryState.Low1)]
    [TestCase("OK", RadselBatteryState.OK)]
    [TestCase("NotUsed", RadselBatteryState.NotUsed)]
    [TestCase("Disconnected", RadselBatteryState.Disconnected)]
    public void ReadBatteryStateTest(string input, RadselBatteryState expectedBatteryState) {
        var result = ReadBatteryState(input);
        Assert.That(result, Is.EqualTo(expectedBatteryState));
    }
    [TestCase(RadselBatteryState.OK, 20, "{\"State\":\"OK\",\"Charge\":20}")]
    [TestCase(RadselBatteryState.Low1, null, "{\"State\":\"Low1\"}")]
    public void ReadBatteryTest(RadselBatteryState expectedBatteryState, int? expectedBatteryCharge, string input) {
        var inputJson = JToken.Parse(input);
        var result = ReadBattery(inputJson);
        Assert.That(result.State, Is.EqualTo(expectedBatteryState));
        Assert.That(result.Charge, Is.EqualTo(expectedBatteryCharge));
    }
    [TestCase(RadselCaseState.Opened, "{\"Case\":1}")]
    [TestCase(RadselCaseState.Closed, "{\"Case\":0}")]
    [TestCase(null, "{}")]
    public void ReadCaseStateTest(RadselCaseState? expectedCaseState, string input) {
        var inputJson = JToken.Parse(input);
        var result = ReadCaseState(inputJson);
        Assert.That(result, Is.EqualTo(expectedCaseState));
    }
    [TestCase(12.3f, "{\"Power\":12.3}")]
    [TestCase(12f, "{\"Power\":12}")]
    [TestCase(null, "{\"Power\":\"Off\"}")]
    public void ReadPowerTest(float? expectedPower, string input) {
        var inputJson = JToken.Parse(input);
        var result = ReadPower(inputJson);
        Assert.That(result, Is.EqualTo(expectedPower));
    }
    [TestCase(22, "{\"Temp\":22}")]
    [TestCase(null, "{\"Temp\":\"NotValid\"}")]
    public void ReadTempTest(int? expectedTemp, string input) {
        var inputJson = JToken.Parse(input);
        var result = ReadTemp(inputJson);
        Assert.That(result, Is.EqualTo(expectedTemp));
    }
    [TestCase(22f, "{\"Balance\":22}")]
    [TestCase(22.5f, "{\"Balance\":22.5}")]
    [TestCase(null, "{\"Balance\":\"NotValid\"}")]
    public void ReadBalanceTest(float? expectedBalance, string input) {
        var inputJson = JToken.Parse(input);
        var result = ReadBalance(inputJson);
        Assert.That(result, Is.EqualTo(expectedBalance));
    }
    [TestCase(true, 0, "{\"Active\":1,\"Voltage\":0}")]
    [TestCase(false, 4095, "{\"Active\":0,\"Voltage\":4095}")]
    public void ReadInputTest(bool expectedActiveState, int expectedVoltage, string input) {
        var inputJson = JToken.Parse(input);
        var result = ReadInput(inputJson);
        Assert.That(result.IsActive, Is.EqualTo(expectedActiveState));
        Assert.That(result.Voltage, Is.EqualTo(expectedVoltage));
    }
    [TestCase("Arm", RadselPartitionState.Arm)]
    [TestCase("Disarm", RadselPartitionState.Disarm)]
    [TestCase("Protect", RadselPartitionState.Protect)]
    public void ReadPartitionStateTest(string input, RadselPartitionState expectedPartitionState) {
        var result = ReadPartitionState(input);
        Assert.That(result, Is.EqualTo(expectedPartitionState));
    }
    [TestCase(1, true)]
    [TestCase(0, false)]
    public void ReadOutputTest(int input, bool expectedState) {
        var result = ReadOutput(input);
        Assert.That(result.IsActive, Is.EqualTo(expectedState));
    }
    [TestCase("CCU825", "HOME+", "E01.1", 16, 4, "10.02", "02.02", "01.02", "Aug 31 2015", "RUS", "1414FD09535605154EF8C306F5043213", "869158123877455", 17, "{\"DeviceType\":\"CCU825\",\"DeviceMod\":\"HOME+\",\"ExtBoard\":\"E01.1\",\"InputsCount\":16,\"PartitionsCount\":4,\"HwVer\":\"10.02\",\"FwVer\":\"02.02\",\"BootVer\":\"01.02\",\"FwBuildDate\":\"Aug 31 2015\",\"CountryCode\":\"RUS\",\"Serial\":\"1414FD09535605154EF8C306F5043213\",\"IMEI\":\"869158123877455\",\"uGuardVerCode\":17}")]
    public void ReadDeviceInfoTest(string dT, string dM, string eB, int iC, int pC, string hV, string fV, string bV, string fbD, string cc, string serial, string imei, int uGuardVerCode, string input) {
        var inputJson = JToken.Parse(input);
        var result = ReadDeviceInfo(inputJson);
        Assert.That(result.DeviceType, Is.EqualTo(dT));
        Assert.That(result.DeviceMod, Is.EqualTo(dM));
        Assert.That(result.ExtBoard, Is.EqualTo(eB));
        Assert.That(result.InputsCount, Is.EqualTo(iC));
        Assert.That(result.PartitionsCount, Is.EqualTo(pC));
        Assert.That(result.HwVer, Is.EqualTo(hV));
        Assert.That(result.FwVer, Is.EqualTo(fV));
        Assert.That(result.BootVer, Is.EqualTo(bV));
        Assert.That(result.FwBuildDate, Is.EqualTo(fbD));
        Assert.That(result.CountryCode, Is.EqualTo(cc));
        Assert.That(result.Serial, Is.EqualTo(serial));
        Assert.That(result.IMEI, Is.EqualTo(imei));
        Assert.That(result.uGuardVerCode, Is.EqualTo(uGuardVerCode));
    }
    [TestCase("[\"+79109451234\",\"Петров\",\"S8\",\"\",\"\",\"\",\"OFF\",\"ON\",\"\"]", "+79109451234", "Петров", "S8", null, null, null, "OFF", "ON", null)]
    public void ReadGateUserConfigTest(string input, string phone, string username, params string?[] actions) {
        var inputJson = JArray.Parse(input);
        var result = ReadGateUserConfig(inputJson);
        Assert.That(result.Phone, Is.EqualTo(phone));
        Assert.That(result.Username, Is.EqualTo(username));
        Assert.That(result.OutputActions, Has.Length.EqualTo(actions.Length));
        for (var i = 0; i < actions.Length; ++i) {
            RadselGateOutputAction? expectedAction;
            if (actions[i] == "ON") {
                expectedAction = new RadselGateOutputActionPower(true);
            } else if (actions[i] == "OFF") {
                expectedAction = new RadselGateOutputActionPower(false);
            } else if (actions[i] == null) {
                expectedAction = null;
            } else if (actions[i]!.StartsWith("S") == true) {
                expectedAction = new RadselGateOutputActionScenario(actions[i]!);
            } else {
                throw new ArgumentException("Invalid input arguments");
            }
            Assert.That(result.OutputActions[i], Is.EqualTo(expectedAction));
        }
    }
    [Test]
    [Repeat(3)]
    public void SerialieGateUserConfigTest() {
        var config = GenerateUserConfig();
        var configSerialized = Serialize(config);
        var readConfig = ReadGateUserConfig(configSerialized);
        Assert.That(readConfig.Phone, Is.EqualTo(config.Phone));
        Assert.That(readConfig.Username, Is.EqualTo(config.Username));
        Assert.That(readConfig.OutputActions, Is.EqualTo(config.OutputActions).AsCollection);
    }
    [TestCase("[1,\"2021-06-04T21:07:46\",\"+79109451234\"]", 1, "2021-06-04T21:07:46", "+79109451234")]
    public void ReadGateRecordTest(string input, int index, string dateTimeStr, string phone) {
        var inputJson = JArray.Parse(input);
        var result = ReadGateRecord(inputJson);
        var expectedDateTime = DateTime.Parse(dateTimeStr);
        Assert.That(result.Index, Is.EqualTo(index));
        Assert.That(result.DateTime, Is.EqualTo(expectedDateTime));
        Assert.That(result.Phone, Is.EqualTo(phone));
    }
    [TestCase("{\"Records\":[[1,\"2021-06-04T21:07:46\",\"+79109451234\"],[2,\"2021-06-04T21:08:00\",\"+79109454321\"]],\"Last\":true}", 2, true)]
    public void ReadGateRecordScrollTest(string input, int numberOfRecords, bool? last) {
        var inputJson = JToken.Parse(input);
        var result = ReadGateRecordScroll(inputJson);
        Assert.That(result.Records, Has.Length.EqualTo(numberOfRecords));
        Assert.That(result.Last, Is.EqualTo(last));
    }
}

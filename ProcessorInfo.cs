```
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

internal class Program
{
    class processorIdentifierInternal
    {
        public string Manufacturer { get; set; }
        public string Vendor { get; set; }
        public uint Family { get; set; }
        public uint Model { get; set; }
        public uint Stepping { get; set; }
    }

    [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetEnvironmentVariable(string lpName, StringBuilder lpValue, int size);

    public static string GetEnvironmentVariable(string variable)
    {
        if (variable == null)
        {
            throw new ArgumentNullException("variable");
        }
        new EnvironmentPermission(EnvironmentPermissionAccess.Read, variable).Demand();
        StringBuilder stringBuilder = new StringBuilder(128);
        int environmentVariable = GetEnvironmentVariable(variable, stringBuilder, stringBuilder.Capacity);
        if (environmentVariable == 0 && Marshal.GetLastWin32Error() == 203)
        {
            return null;
        }
        while (environmentVariable > stringBuilder.Capacity)
        {
            stringBuilder.Capacity = environmentVariable;
            stringBuilder.Length = 0;
            environmentVariable = GetEnvironmentVariable(variable, stringBuilder, stringBuilder.Capacity);
        }
        return stringBuilder.ToString();
    }
    private enum MicroArchitecture
    {
        Airmont,
        AlderLake,
        Atom,
        ArrowLake, // Gen. 15 (0xC6, -H = 0xC5)
        Broadwell,
        CannonLake,
        CometLake,
        Core,
        Goldmont,
        GoldmontPlus,
        Haswell,
        IceLake,
        IvyBridge,
        JasperLake,
        KabyLake,
        LunarLake,
        Nehalem,
        NetBurst,
        MeteorLake,
        PantherLake,
        RocketLake,
        SandyBridge,
        Silvermont,
        Skylake,
        TigerLake,
        Tremont,
        RaptorLake,
        SapphireRapids,
        ElkhartLake,
        Unknown
    }

    static MicroArchitecture _microArchitecture;

    static void IntelCpu(uint _family, uint _model)
    {
        switch (_family)
        {
            case 0x06:
                {
                    switch (_model)
                    {
                        case 0x0F: // Intel Core 2 (65nm)
                            _microArchitecture = MicroArchitecture.Core;
                            ////tjMax = _stepping switch
                            //{
                            //    // B2
                            //    0x06 => _coreCount switch
                            //    {
                            //        2 => Floats(80 + 10),
                            //        4 => Floats(90 + 10),
                            //        _ => Floats(85 + 10)
                            //    },
                            //    // G0
                            //    0x0B => Floats(90 + 10),
                            //    // M0
                            //    0x0D => Floats(85 + 10),
                            //    _ => Floats(85 + 10)
                            //};
                            break;

                        case 0x17: // Intel Core 2 (45nm)
                            _microArchitecture = MicroArchitecture.Core;
                            ////tjMax = Floats(100);
                            break;

                        case 0x1C: // Intel Atom (45nm)
                            _microArchitecture = MicroArchitecture.Atom;
                            ////tjMax = _stepping switch
                            //{
                            //    // C0
                            //    0x02 => Floats(90),
                            //    // A0, B0
                            //    0x0A => Floats(100),
                            //    _ => Floats(90)
                            //};
                            break;

                        case 0x1A: // Intel Core i7 LGA1366 (45nm)
                        case 0x1E: // Intel Core i5, i7 LGA1156 (45nm)
                        case 0x1F: // Intel Core i5, i7
                        case 0x25: // Intel Core i3, i5, i7 LGA1156 (32nm)
                        case 0x2C: // Intel Core i7 LGA1366 (32nm) 6 Core
                        case 0x2E: // Intel Xeon Processor 7500 series (45nm)
                        case 0x2F: // Intel Xeon Processor (32nm)
                            _microArchitecture = MicroArchitecture.Nehalem;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x2A: // Intel Core i5, i7 2xxx LGA1155 (32nm)
                        case 0x2D: // Next Generation Intel Xeon, i7 3xxx LGA2011 (32nm)
                            _microArchitecture = MicroArchitecture.SandyBridge;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x3A: // Intel Core i5, i7 3xxx LGA1155 (22nm)
                        case 0x3E: // Intel Core i7 4xxx LGA2011 (22nm)
                            _microArchitecture = MicroArchitecture.IvyBridge;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x3C: // Intel Core i5, i7 4xxx LGA1150 (22nm)
                        case 0x3F: // Intel Xeon E5-2600/1600 v3, Core i7-59xx
                                   // LGA2011-v3, Haswell-E (22nm)
                        case 0x45: // Intel Core i5, i7 4xxxU (22nm)
                        case 0x46:
                            _microArchitecture = MicroArchitecture.Haswell;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x3D: // Intel Core M-5xxx (14nm)
                        case 0x47: // Intel i5, i7 5xxx, Xeon E3-1200 v4 (14nm)
                        case 0x4F: // Intel Xeon E5-26xx v4
                        case 0x56: // Intel Xeon D-15xx
                            _microArchitecture = MicroArchitecture.Broadwell;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x36: // Intel Atom S1xxx, D2xxx, N2xxx (32nm)
                            _microArchitecture = MicroArchitecture.Atom;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x37: // Intel Atom E3xxx, Z3xxx (22nm)
                        case 0x4A:
                        case 0x4D: // Intel Atom C2xxx (22nm)
                        case 0x5A:
                        case 0x5D:
                            _microArchitecture = MicroArchitecture.Silvermont;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x4E:
                        case 0x5E: // Intel Core i5, i7 6xxxx LGA1151 (14nm)
                        case 0x55: // Intel Core X i7, i9 7xxx LGA2066 (14nm)
                            _microArchitecture = MicroArchitecture.Skylake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x4C: // Intel Airmont (Cherry Trail, Braswell)
                            _microArchitecture = MicroArchitecture.Airmont;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x8E: // Intel Core i5, i7 7xxxx (14nm) (Kaby Lake) and 8xxxx (14nm++) (Coffee Lake)
                        case 0x9E:
                            _microArchitecture = MicroArchitecture.KabyLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x5C: // Goldmont (Apollo Lake)
                        case 0x5F: // (Denverton)
                            _microArchitecture = MicroArchitecture.Goldmont;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x7A: // Goldmont plus (Gemini Lake)
                            _microArchitecture = MicroArchitecture.GoldmontPlus;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x66: // Intel Core i3 8xxx (10nm) (Cannon Lake)
                            _microArchitecture = MicroArchitecture.CannonLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x7D: // Intel Core i3, i5, i7 10xxx (10nm) (Ice Lake)
                        case 0x7E:
                        case 0x6A: // Ice Lake server
                        case 0x6C:
                            _microArchitecture = MicroArchitecture.IceLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0xA5:
                        case 0xA6: // Intel Core i3, i5, i7 10xxxU (14nm)
                            _microArchitecture = MicroArchitecture.CometLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x86: // Tremont (10nm) (Elkhart Lake, Skyhawk Lake)
                            _microArchitecture = MicroArchitecture.Tremont;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x8C: // Tiger Lake (Intel 10 nm SuperFin, Gen. 11)
                        case 0x8D:
                            _microArchitecture = MicroArchitecture.TigerLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x97: // Alder Lake (Intel 7 (10ESF), Gen. 12)
                        case 0x9A: // Alder Lake-L (Intel 7 (10ESF), Gen. 12)
                        case 0xBE: // Alder Lake-N (Intel 7 (10ESF), Gen. 12)
                            _microArchitecture = MicroArchitecture.AlderLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0xB7: // Raptor Lake (Intel 7 (10ESF), Gen. 13)
                        case 0xBA: // Raptor Lake-P (Intel 7 (10ESF), Gen. 13)
                        case 0xBF: // Raptor Lake-N (Intel 7 (10ESF), Gen. 13)
                            _microArchitecture = MicroArchitecture.RaptorLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0xAC: // Meteor Lake (Intel 4, TSMC N5/N6, Gen. 14)
                        case 0xAA: // Meteor Lake-L (Intel 4, TSMC N5/N6, Gen. 14)
                            _microArchitecture = MicroArchitecture.MeteorLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x9C: // Jasper Lake (10nm)
                            _microArchitecture = MicroArchitecture.JasperLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0xA7: // Intel Core i5, i6, i7 11xxx (14nm) (Rocket Lake)
                            _microArchitecture = MicroArchitecture.RocketLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0xB5: // Intel Core Ultra 5/7 200 Series ArrowLake-U
                        case 0xC5: // Intel Core Ultra 9 200 Series ArrowLake
                        case 0xC6: // Intel Core Ultra 7 200 Series ArrowLake
                            _microArchitecture = MicroArchitecture.ArrowLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0xBD: // Intel Core Ultra 5/7 200 Series LunarLake
                            _microArchitecture = MicroArchitecture.LunarLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x8F: // Intel Xeon W5-3435X // SapphireRapids 
                            _microArchitecture = MicroArchitecture.SapphireRapids;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0x96: // Intel Celeron ElkhartLake 
                            _microArchitecture = MicroArchitecture.ElkhartLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        case 0xCC: // Intel Panther Lake.
                            _microArchitecture = MicroArchitecture.PantherLake;
                            //tjMax = GetTjMaxFromMsr();
                            break;

                        default:
                            _microArchitecture = MicroArchitecture.Unknown;
                            //tjMax = Floats(100);
                            break;
                    }
                }

                break;
            case 0x0F:
                switch (_model)
                {
                    case 0x00: // Pentium 4 (180nm)
                    case 0x01: // Pentium 4 (130nm)
                    case 0x02: // Pentium 4 (130nm)
                    case 0x03: // Pentium 4, Celeron D (90nm)
                    case 0x04: // Pentium 4, Pentium D, Celeron D (90nm)
                    case 0x06: // Pentium 4, Pentium D, Celeron D (65nm)
                        _microArchitecture = MicroArchitecture.NetBurst;
                        //tjMax = Floats(100);
                        break;

                    default:
                        _microArchitecture = MicroArchitecture.Unknown;
                        //tjMax = Floats(100);
                        break;
                }

                break;
            default:
                _microArchitecture = MicroArchitecture.Unknown;
                //tjMax = Floats(100);
                break;
        }
    }


    static void Main(string[] args)
    {
        var processorIdentifier = GetEnvironmentVariable("PROCESSOR_IDENTIFIER");

        processorIdentifierInternal processorIdentifierInternal = new processorIdentifierInternal();
        var split1 = processorIdentifier.Split(',');
        if (split1.Length == 2 && split1[1].Contains("GenuineIntel"))
        {
            processorIdentifierInternal.Manufacturer = split1[1];

        }

        var split = processorIdentifier.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
        var strs = new List<string>();
        foreach (var split2 in split)
        {
            var split3 = split2.Replace(",", "");
            strs.Add(split3);
        }

        for (int i = 0; i < strs.Count; i++)
        {
            if (strs[i] == "Intel64" || strs[i] == "AMD64")
            {
                processorIdentifierInternal.Vendor = strs[i];

            }
            else if (strs[i].StartsWith("Family"))
            {
                /*processorIdentifierInternal.Family = */
                uint.TryParse(strs[i + 1], out var result);
                processorIdentifierInternal.Family = result;
            }
            else if (strs[i].StartsWith("Model"))
            {
                uint.TryParse(strs[i + 1], out var result);
                processorIdentifierInternal.Model = result;
            }
            else if (strs[i].StartsWith("Stepping"))
            {
                uint.TryParse(strs[i + 1], out var result);
                processorIdentifierInternal.Stepping = result;
            }
        }

        IntelCpu(processorIdentifierInternal.Family, processorIdentifierInternal.Model);
        Console.WriteLine(_microArchitecture.ToString());
        Console.WriteLine(processorIdentifierInternal.Manufacturer);
        Console.WriteLine(processorIdentifierInternal.Vendor);

        Console.ReadLine();
    }
}

```

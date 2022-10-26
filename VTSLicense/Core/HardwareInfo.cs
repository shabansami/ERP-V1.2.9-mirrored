﻿using System;
using System.Collections.Generic;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace VTSLicense.Core
{
    internal class HardwareInfo
    {
        /// <summary>
        /// Get volume serial number of drive C
        /// </summary>
        /// <returns></returns>
        private static string GetDiskVolumeSerialNumber()
        {
            try
            {
                var _disk = new ManagementObject(@"Win32_LogicalDisk.deviceid=""c:""");
                _disk.Get();
                return _disk["VolumeSerialNumber"].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get CPU ID
        /// </summary>
        /// <returns></returns>
        private static string GetProcessorId()
        {
            try
            {
                var _mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                var _mbsList = _mbs.Get();
                var _id = string.Empty;
                foreach (ManagementObject _mo in _mbsList)
                {
                    _id = _mo["ProcessorId"].ToString();
                    break;
                }

                return _id;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get motherboard serial number
        /// </summary>
        /// <returns></returns>
        private static string GetMotherboardID()
        {
            try
            {
                var _mbs = new ManagementObjectSearcher("Select SerialNumber From Win32_BaseBoard");
                var _mbsList = _mbs.Get();
                var _id = string.Empty;
                foreach (ManagementObject _mo in _mbsList)
                {
                    _id = _mo["SerialNumber"].ToString();
                    break;
                }

                return _id;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static IEnumerable<string> SplitInParts(string input, int partLength)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", "partLength");

            for (var i = 0; i < input.Length; i += partLength)
                yield return input.Substring(i, Math.Min(partLength, input.Length - i));
        }

        /// <summary>
        /// Combine CPU ID, Disk C Volume Serial Number and Motherboard Serial Number as device Id
        /// </summary>
        /// <returns></returns>
        public static string GenerateUID(string appName)
        {
            //Combine the IDs and get bytes
            var _id = string.Concat(appName, GetProcessorId(), GetMotherboardID(), GetDiskVolumeSerialNumber());
            var _byteIds = Encoding.UTF8.GetBytes(_id);

            //Use MD5 to get the fixed length checksum of the ID string
            var _md5 = new MD5CryptoServiceProvider();
            var _checksum = _md5.ComputeHash(_byteIds);

            //Convert checksum into 4 ulong parts and use Base36 to encode both
            var _part1Id = Base36.Encode(BitConverter.ToUInt32(_checksum, 0));
            var _part2Id = Base36.Encode(BitConverter.ToUInt32(_checksum, 4));
            var _part3Id = Base36.Encode(BitConverter.ToUInt32(_checksum, 8));
            var _part4Id = Base36.Encode(BitConverter.ToUInt32(_checksum, 12));

            //Concat these 4 part into one string
            return string.Format("{0}-{1}-{2}-{3}", _part1Id, _part2Id, _part3Id, _part4Id);
        }

        public static byte[] GetUIDInBytes(string UID)
        {
            //Split 4 part Id into 4 ulong
            var _ids = UID.Split('-');

            if (_ids.Length != 4) throw new ArgumentException("Wrong UID");

            //Combine 4 part Id into one byte array
            var _value = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(Base36.Decode(_ids[0])), 0, _value, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(Base36.Decode(_ids[1])), 0, _value, 8, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(Base36.Decode(_ids[2])), 0, _value, 16, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(Base36.Decode(_ids[3])), 0, _value, 24, 8);

            return _value;
        }

        public static bool ValidateUIDFormat(string UID)
        {
            if (!string.IsNullOrWhiteSpace(UID))
            {
                var _ids = UID.Split('-');

                return _ids.Length == 4;
            }
            else
            {
                return false;
            }
        }
    }
}
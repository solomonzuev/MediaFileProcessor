﻿using System.Runtime.InteropServices;
using MediaFileProcessor.Models.Enums;
namespace MediaFileProcessor.Extensions;

/// <summary>
/// A class that contains extension methods for working with file data
/// </summary>
public static class FileDataExtensions
{
    /// <summary>
    /// Checks if the specified string value exists in the specified enum type T.
    /// </summary>
    /// <typeparam name="T">The enum type to search for the value in.</typeparam>
    /// <param name="value">The string value to search for.</param>
    /// <returns>True if the value exists in the enum type T, false otherwise.</returns>
    public static bool ExistsInEnum<T>(this string value) where T : Enum
    {
        return Enum.GetNames(typeof(T)).Any(i => "." + i.Replace("_", "").ToLower() == value.ToLower());
    }

    /// <summary>
    /// Converts the specified file name to a stream.
    /// </summary>
    /// <param name="fileName">The file name to convert.</param>
    /// <param name="mode">The mode to open the file stream with. Defaults to FileMode.Open.</param>
    /// <returns>A FileStream representing the specified file name.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file specified in <paramref name="fileName"/> does not exist.</exception>
    public static FileStream ToFileStream(this string fileName, FileMode mode = FileMode.Open)
    {
        if(!File.Exists(fileName))
            throw new FileNotFoundException($"File not found: {fileName}");

        return new FileStream(fileName, mode);
    }

    /// <summary>
    /// Gets the extension of the specified file name.
    /// </summary>
    /// <param name="fileName">The file name to get the extension for.</param>
    /// <returns>The extension of the specified file name.</returns>
    public static string GetExtension(this string fileName)
    {
        return Path.GetExtension(fileName).Replace(".", "");
    }

    /// <summary>
    /// Converts the specified file name to an array of bytes.
    /// </summary>
    /// <param name="fileName">The file name to convert.</param>
    /// <returns>An array of bytes representing the contents of the file specified in <paramref name="fileName"/>.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file specified in <paramref name="fileName"/> does not exist.</exception>
    public static byte[] ToBytes(this string fileName)
    {
        if(!File.Exists(fileName))
            throw new FileNotFoundException($"File not found: {fileName}");

        return File.ReadAllBytes(fileName);
    }

    /// <summary>
    /// Converts the specified byte array to a memory stream.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>A MemoryStream representing the specified byte array.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="bytes"/> is an empty array.</exception>
    public static MemoryStream ToFileStream(this byte[] bytes)
    {
        if(bytes.Length is 0)
            throw new ArgumentException("Byte array is empty");

        return new MemoryStream(bytes);
    }

    /// <summary>
    /// Writes the specified byte array to a file.
    /// </summary>
    /// <param name="bytes">The byte array to write to a file.</param>
    /// <param name="fileName">The name of the file to create or overwrite.</param>
    public static void ToFile(this byte[] bytes, string fileName)
    {
        using (var output = new FileStream(fileName, FileMode.Create))
            output.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Converts a pipe name to the appropriate pipe directory format for the current operating system.
    /// </summary>
    /// <param name="pipeName">The name of the pipe</param>
    /// <returns>A string representing the pipe directory</returns>
    public static string ToPipeDir(this string pipeName)
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return $@"\\.\pipe\{pipeName}";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return pipeName;

        // Throw an exception if the operating system cannot be recognized
        throw new Exception("Operating System not supported");
    }

    /// <summary>
    /// Writes the contents of the specified stream to a file.
    /// </summary>
    /// <param name="stream">The stream to write to a file.</param>
    /// <param name="fileName">The name of the file to create or overwrite.</param>
    public static void ToFile(this Stream stream, string fileName)
    {
        using (var output = new FileStream(fileName, FileMode.Create))
            stream.CopyTo(output);
    }

    /// <summary>
    /// Gets the format type of a file based on its file extension.
    /// </summary>
    /// <param name="fileName">The name of the file to get the format type of.</param>
    /// <returns>The format type of the file.</returns>
    /// <exception cref="Exception">Thrown if the file extension of <paramref name="fileName"/> could not be recognized.</exception>
    public static FileFormatType GetFileFormatType(this string fileName)
    {
        var ext = Path.GetExtension(fileName);

        if(char.IsDigit(ext[1]))
            ext = "_" + ext;

        if (Enum.TryParse(ext.ToUpper().Replace(".", ""), out FileFormatType output))
            return output;

        throw new Exception("The file extension could not be recognized");
    }

    /// <summary>
    /// Concatenates multiple arrays of bytes into a single array of bytes.
    /// </summary>
    /// <param name="onlyNotDefaultArrays">A flag indicating whether only arrays that contain non-zero bytes should be concatenated.</param>
    /// <param name="arrays">The arrays of bytes to concatenate.</param>
    /// <returns>A single array of bytes that is the result of concatenating the input arrays.</returns>
    public static byte[] ConcatByteArrays(bool onlyNotDefaultArrays, params byte[][] arrays)
    {
        if(onlyNotDefaultArrays)
            arrays = arrays.Where(x => x.Any(y => y != 0))
                           .ToArray();

        var z = new byte[arrays.Sum(x => x.Length)];

        var lengthSum = 0;

        foreach (var array in arrays)
        {
            Buffer.BlockCopy(array, 0, z, lengthSum, array.Length * sizeof(byte));
            lengthSum += array.Length * sizeof(byte);
        }

        return z;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WildCompile.Models;
using System.IO;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WildCompile.Controllers
{

    [Route("api/[controller]")]
    public class CompileController : Controller
    {
        private const string endResult = @"C:\Compile\Main.exe";
        private readonly string cSharpEXE = $@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe";

        // GET: api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CompileInput compile)
        {
            if (compile.pass != "PASS")
            {
                return BadRequest();
            }
            compile.filename = @"C:\Compile\Main.cs";
            if (System.IO.File.Exists(endResult))
            {
                System.IO.File.Delete(endResult);
            }
            if (System.IO.File.Exists(compile.filename))
            {
                System.IO.File.Delete(compile.filename);
            }           
            
            using (StreamWriter writer = System.IO.File.CreateText(compile.filename))
            {
                await writer.WriteAsync(compile.code);
                await writer.FlushAsync();
            }
            string filesToCompile = "";
            filesToCompile += $"/out:{endResult}";
            foreach (string file in new string[] { compile.filename })
            {
                filesToCompile += " " + file;
            }
            // TODO: Throw error if comile failss
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = cSharpEXE;
            p.StartInfo.Arguments = filesToCompile;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            if (output.Contains("error"))
            {
                return BadRequest(output.Substring(423));
            }

            Process userRun = new Process();
            userRun.StartInfo.FileName = "C:\\Compile\\Runner\\Runner.exe"; //TODO:Enter file name
            //userRun.StartInfo.Arguments = $"{compile.key} C:\\Compile\\Main.exe ws://ec2-54-234-53-50.compute-1.amazonaws.com/ws";
            //userRun.StartInfo.Arguments = $"{compile.key} C:\\Compile\\Main.exe ws://localhost:62299/ws";
            //ws://ec2-54-234-53-50.compute-1.amazonaws.com/ws
            userRun.StartInfo.Arguments = $"{compile.key} C:\\Compile\\Main.exe ws://ec2-54-234-53-50.compute-1.amazonaws.com/ws";
            userRun.Start();

            return Ok();
        }
    }
}

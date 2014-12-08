open FSharp.Data;

type LogProvider = XmlProvider<"sample-log.xml">
let provider path = LogProvider.Parse(path) 


[<EntryPoint>]
let main argv = 
    let path = System.Environment.GetEnvironmentVariable("PATH").Split(';') |> Seq.map(fun x -> System.IO.Path.Combine(x, "svn.exe")) |> Seq.where System.IO.File.Exists|> Seq.head

    let procInfo = System.Diagnostics.ProcessStartInfo(path)
    procInfo.Arguments <- "log --xml"
    procInfo.WorkingDirectory <- System.Environment.CurrentDirectory
    procInfo.RedirectStandardOutput <- true
    procInfo.CreateNoWindow <- true
    procInfo.UseShellExecute <- false
    
    printfn "%A" (procInfo.FileName)
    let proc = new System.Diagnostics.Process()
    proc.StartInfo <- procInfo
    proc.Start() |> ignore

    let log = LogProvider.Parse(proc.StandardOutput.ReadToEnd())
    
    let commitsFrom author = log.Logentries |> Seq.where (fun x -> x.Author = author)

    let authors = [| "marek.kadek"; "martin.kolinek"; "vladimir.pavelka"; "marek.sedlacek"; "robert.herceg"; "branislav.pavelka" |]

    authors |> Seq.map (fun x -> (x, commitsFrom x |> Seq.length ) ) |> Seq.iter (fun (author, commits) -> printfn "%A : %A" author commits)
    System.Console.ReadKey() |> ignore
    0

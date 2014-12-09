open FSharp.Data;
open CommandLine;

type LogProvider = XmlProvider<"sample-log.xml">
let provider path = LogProvider.Parse(path) 


[<EntryPoint>]
let main argv = 
    let options = defaultCommandLineOptions |> parseCommandLineRec (argv |> Array.toList)

    let path = System.Environment.GetEnvironmentVariable("PATH").Split(';') |> Seq.map(fun x -> System.IO.Path.Combine(x, "svn.exe")) |> Seq.where System.IO.File.Exists|> Seq.head

    match options.svn with
    | None -> printfn "SVN repo not set"
    | Some (SvnRepositoryOption repo) ->
        let procInfo = System.Diagnostics.ProcessStartInfo(path)
        let arguments = 
            let def = "log " + repo + " --xml"
            let username = 
                match options.username with
                | Some usern -> "--username " + usern
                |_ -> ""
            let password = 
                match options.password with
                | Some passwd -> "--password " + passwd
                | _ -> ""
            def + username + password
        
        procInfo.Arguments <- arguments
        procInfo.WorkingDirectory <- System.Environment.CurrentDirectory
        procInfo.RedirectStandardOutput <- true
        procInfo.CreateNoWindow <- true
        procInfo.UseShellExecute <- false
    
        let proc = new System.Diagnostics.Process()
        proc.StartInfo <- procInfo
        proc.Start() |> ignore

        let log = LogProvider.Parse(proc.StandardOutput.ReadToEnd())
    
        let commitsFrom author from until = 
            let all = log.Logentries |> Seq.where (fun x -> x.Author = author)
            let fromFiltered = match from with
                | Some date -> all |> Seq.where (fun x -> x.Date >= date)
                | None -> all
            let untilFiltered = match until with
                | Some date -> fromFiltered |> Seq.where (fun x -> x.Date <= date)
                | None -> fromFiltered
            untilFiltered
    
        let authors = [| "marek.kadek"; "martin.kolinek"; "vladimir.pavelka"; "marek.sedlacek"; "robert.herceg"; "branislav.pavelka" |]

        authors 
        |> Seq.map (fun x -> (x, commitsFrom x options.from options.until |> Seq.length ) ) 
        |> Seq.sortBy(fun (_, amount) -> amount) 
        |> Seq.iter (fun (author, commits) -> printfn "%A : %A" author commits)

    System.Console.ReadKey() |> ignore
    0

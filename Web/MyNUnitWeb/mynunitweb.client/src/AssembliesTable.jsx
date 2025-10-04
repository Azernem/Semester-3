import { useEffect, useState } from 'react';
import axios from 'axios';
import ClassesTable from './ClassesTable';

export default function AssembliesTable({ setUploadProgress }) {
    const [history, setHistory] = useState([]);
    const [testing, setTesting] = useState(true);

    function runTestsButtonContent() {
        if (testing) {
            return <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
            </div>
        }
        return "Run Tests";  
    }

    async function handleRunTestsSubmit(event) {
        setTesting(true);
        setUploadProgress(0);
        event.preventDefault();
        const url = 'https://localhost:7232/runtests/GetTestResults';
        try {
            const response = await axios.get(url);
            console.log(response);
            setHistory(response.data);
        } catch (error) {
            console.log("Error testing: ", error);
        } finally {
            setTesting(false);
        }
    }

    useEffect(() => {
        axios.get('https://localhost:7232/runtests/gethistory')
            .then((response) => {
                console.log(response);
                setHistory(response.data);
                setTesting(false);
            })
            .catch((error) => {
                console.log("Error fetching history: ", error);  
            });
    }, [])

    return (
        <>
            <form onSubmit={handleRunTestsSubmit}>
                <button type="submit" disabled={testing}>{runTestsButtonContent()}</button>  {}
            </form>
            <table className="table">
                <thead>
                    <tr>
                        <th scope="col">Assembly name</th>
                        <th scope="col">Passed</th>
                        <th scope="col">Failed</th>
                        <th scope="col">Ignored</th>
                    </tr>
                </thead>
                <tbody>
                    {history.map(assembly =>
                        <>
                            <tr key={assembly.assemblyResultId} data-bs-toggle="collapse" data-bs-target={"#m" + assembly.assemblyResultId}>
                                <td>
                                    <button type="button">
                                        {assembly.assemblyName}
                                    </button>
                                </td>
                                <td>{assembly.passed}</td>
                                <td>{assembly.failed}</td>
                                <td>{assembly.ignored}</td>
                            </tr>
                            <tr className="collapse" id={"m" + assembly.assemblyResultId}>
                                <td colSpan="4">  {}
                                    <ClassesTable classResults={assembly.classResults} />
                                </td>
                            </tr>
                        </>
                    )}
                </tbody>
            </table>
        </>
    );
}

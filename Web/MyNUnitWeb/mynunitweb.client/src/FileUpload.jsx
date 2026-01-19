import { useState } from 'react';
import axios from 'axios';
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.bundle.min";

export default function FileUpload({ uploadProgress, setUploadProgress }) {
    const [files, setFiles] = useState([]);
    const [uploading, setUploading] = useState(false);
    const [error, setError] = useState(null); 

    function handleMultipleChange(event) {
        setFiles([...event.target.files]);
    }

    function handleMultipleSubmit(event) {
        if (files.length === 0) {
            setError('Please select files to upload'); 
            return;
        }
        setUploading(true);
        setError(null); 
        event.preventDefault();
        const url = 'https://localhost:7232/runtests/upload';
        const formData = new FormData();
        files.forEach((file) => {
            formData.append(`files`, file);
        });

        const config = {
            headers: {
                'content-type': 'multipart/form-data',
            },
            onUploadProgress: function (progressEvent) {
                const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total);
                setUploadProgress(percentCompleted);
            }
        };
        axios.post(url, formData, config)
            .then((response) => {
                console.log(response.data);
                setUploading(false);
                setError(null);
            })
            .catch((error) => {
                console.error("Error uploading files: ", error);
                setUploading(false);
                setError('Error uploading files. Please try again later.');
            });
    }

    function uploadButtonContent() {
        if (uploading) {
            return <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
            </div>;
        }
        return "Upload";
    }

    return (
        <>
            <div>
                <form onSubmit={handleMultipleSubmit} lang="en">
                    <input type="file" multiple onChange={handleMultipleChange} />
                    <button type="submit" className="btn btn-primary">{uploadButtonContent()}</button>
                    {error && <div className="alert alert-danger mt-2">{error}</div>} {}
                    <progress value={uploadProgress} max="100" className="w-100 mt-2"></progress>
                </form>
            </div>
        </>
    );
}

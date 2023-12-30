import { useEffect, useState } from "react"
import { getBooks } from "../apis/BookApi"
import FormControl from "@mui/material/FormControl"
import InputLabel from "@mui/material/InputLabel"
import Select from "@mui/material/Select"
import MenuItem from "@mui/material/MenuItem"




const Book = () => {

  const [books, setBooks] = useState<string[]>([])
  const [selectedBook, setSelectedBook] = useState<string>('')

  useEffect(() => {
    const fetchBooks = async () => {
      const retrievedBooks = await getBooks()
      setBooks(retrievedBooks)
    }
    fetchBooks()  
  }, [])

  return (
    <>
      <Select
        labelId="demo-simple-select-label"
        id="demo-simple-select"
        value={selectedBook}
        label="Select Book"
        onChange={(e) => setSelectedBook(e.target.value)}
      >
        {books.map(book => <MenuItem value={book}>{book}</MenuItem>)}
      </Select>
    </>
  )
}

export default Book

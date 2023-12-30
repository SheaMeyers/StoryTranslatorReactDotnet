import { useEffect, useState } from "react"
import { getBooks } from "../apis/BookApi"
import Select from "@mui/material/Select"
import MenuItem from "@mui/material/MenuItem"
import "../styling/Book.css"




const Book = () => {

  const [books, setBooks] = useState<string[]>([])
  const [selectedBook, setSelectedBook] = useState<string>('')
  const [translateFrom, setTranslateFrom] = useState<string>('')
  const [translateTo, setTranslateTo] = useState<string>('')

  const languages = ['English', 'Spanish', 'French', 'German']

  useEffect(() => {
    const fetchBooks = async () => {
      const retrievedBooks = await getBooks()
      setBooks(retrievedBooks)
    }
    fetchBooks()  
  }, [])

  return (
    <div className="BookSelectorBar">
      <Select
        labelId="select-book-label"
        id="select-book"
        value={selectedBook}
        label="Select Book"
        className="BookSelector"
        onChange={(e) => setSelectedBook(e.target.value)}
      >
        {books.map(book => <MenuItem value={book}>{book}</MenuItem>)}
      </Select>
      <Select
        labelId="translate-from-label"
        id="translate-from"
        value={translateFrom}
        label="Select Book"
        className="BookSelector"
        onChange={(e) => setTranslateFrom(e.target.value)}
      >
        {languages.map(language => <MenuItem value={language}>{language}</MenuItem>)}
      </Select>
      <Select
        labelId="translate-to-label"
        id="translate-to"
        value={translateTo}
        label="Select Book"
        className="BookSelector"
        onChange={(e) => setTranslateTo(e.target.value)}
      >
        {languages.map(language => <MenuItem value={language}>{language}</MenuItem>)}
      </Select>
    </div>
  )
}

export default Book

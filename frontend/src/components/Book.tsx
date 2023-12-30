import { useEffect, useState } from "react"
import { getBooks } from "../apis/BookApi"
import Select from "@mui/material/Select"
import MenuItem from "@mui/material/MenuItem"
import "../styling/Book.css"
import TextField from "@mui/material/TextField"
import { Paragraph } from "../types"
import { getFirstParagraph } from "../apis/ParagraphsApi"
import Popover from "@mui/material/Popover"



const Book = () => {

  const languages = ['English', 'Spanish', 'French', 'German']

  const [books, setBooks] = useState<string[]>([])
  const [selectedBook, setSelectedBook] = useState<string>('')
  const [translateFromSelector, setTranslateFromSelector] = useState<string>('')
  const [translateToSelector, setTranslateToSelector] = useState<string>('')
  const [isPopoverOpen, setIsPopoverOpen] = useState<boolean>(false)
  const [paragraph, setParagraph] = useState<Paragraph>({
    id: -1,
    translateFrom: '',
    translateTo: ''
  })
  

  useEffect(() => {
    const fetchBooks = async () => {
      const retrievedBooks = await getBooks()
      setBooks(retrievedBooks)
    }
    fetchBooks()  
  }, [])

  useEffect(() => {
    const fetchFirstParagraph = async () => {
      const paragraph = await getFirstParagraph(selectedBook, translateFromSelector, translateToSelector)
      setParagraph({
        ...paragraph
      })
    }
    if (selectedBook && translateFromSelector && translateToSelector) {
      fetchFirstParagraph()
    }
  }, [selectedBook, translateFromSelector, translateToSelector])


  return (
    <>
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
          value={translateFromSelector}
          label="Select Book"
          className="BookSelector"
          onChange={(e) => setTranslateFromSelector(e.target.value)}
        >
          {languages.map(language => <MenuItem value={language}>{language}</MenuItem>)}
        </Select>
        <Select
          labelId="translate-to-label"
          id="translate-to"
          value={translateToSelector}
          label="Select Book"
          className="BookSelector"
          onChange={(e) => setTranslateToSelector(e.target.value)}
        >
          {languages.map(language => <MenuItem value={language}>{language}</MenuItem>)}
        </Select>
      </div>
      <div className="TranslationsContainer">
          <TextField
            id="translate-from-text"
            label="Multiline"
            className="TranslationText"
            multiline
            rows={4}
            value={paragraph.translateFrom}
            onClick={() => setIsPopoverOpen(true)}
          />
          <Popover
            id='translate-to-text'
            open={isPopoverOpen}
            anchorEl={document.getElementById('translate-from-text')}
            onClose={() => setIsPopoverOpen(false)}
            anchorOrigin={{
              vertical: 'top',
              horizontal: 'right',
            }}
            transformOrigin={{
              vertical: "bottom",
              horizontal: "left",
            }}
          >
            {paragraph.translateTo}
          </Popover>
        </div>
    </>
  )
}

export default Book

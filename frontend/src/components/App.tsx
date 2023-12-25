import { useEffect, useState } from 'react'
import Header from './Header'


const App = () => {
  const [apiToken, setApiToken] = useState<string>('')

  useEffect(() => {
    setApiToken('')
  }, [])

  return (
    <>
     <Header apiToken={apiToken} /> 
    </>
  )
}

export default App

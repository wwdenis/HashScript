[![master Actions Status](https://github.com/wwdenis/vbparser/workflows/test/badge.svg)](https://github.com/wwdenis/HashScript/actions)

# HashScript
`HashScript` is a simple and powerful scripting language created by @wwdenis and written in .NET.


While there are so many scripting engines, most require a certain learning curve.  
`HashScript` is intended to be lightweight, easy to learn, and still  powerful.

## Roadmap
- Detailed Documentation
- Visual Studio Code Formatter (and other IDE's)
- Custom Data Sources

## Introduction
Everything surrounded with a Hash symbol ( `#` ), is a **Template Field**, or just a `Field`.  
A field is a placeholder for a dynamic content.
`HashScript` gieto be lightweight, easy to learn, and still  powerful.

## Basic Syntax

| Symbol | Description |
| :--: | --------- |
| **#** | Indicates a `Field` |
| **+** | Indicates a `Structured Field` (inside a field) |
| **?** | Indicates a `Conditional Field` (inside a field) |
| **!** | Indicates a `Conditional Field` (inside a field, negate) |
| **.** | Indicates a `Function Field` or a `Value Field` |

## Field Types

| Type | Example | Note |
| -- | -- | -- |
| Content Field | `#Foo#` | A placeholder to render data. |
| Structured Field | `#+Foo# Text #+# `| Renders data below the data structure |
| Confitional Field | `#?Foo# Text #?# `| Renders when condition is `True` (use `!` for `False`) |
| Function Field | `#?.Foo# Text #?# `| Renders when the function `Foo` is `True` |
| Value Field | `The Numers are #.#`| Renders the value of a item in a collection |

## Conditional Fields
`Conditional Field` is very flexible and can work with the followiung data types:
- **Boolean**: `True` or `False`
- **Number**: `True` when greather than Zero
- **Text**: `True` when it has length
- **Collection**: `True` when it has items
- **Object**: `True` when the value is not `null`

## Function Fields
`Function Field` is used to give the Renderer additional data.

For example, in the `ObjectValueProvider` the following functions are defined:
- **.First**: Returns `True` when the item is the first in the colection
- **.Last**: Returns `False` when the item is the last in the colection

### Syntax Example:
```
#+Items#
   #?.First# Foo #?#
#+# 
```

## Examples

<table>


<tr>
<td>

**Type** 

</td>
<td>

**Template** 

</td>
<td>

**Input** 

</td>
<td>

**Output** 

</td>
</tr>
<tr>
  <td>

  **Content Field**

  </td>
  <td>

  ```
  Hi #Name#!!!
  ```

  </td>
  <td>
      
  ```json
  { "Name": "HashScript" }
  ```
  </td>
  <td>
      
  ```
  Hi HashScript!!!         
  ```
  </td>
</tr>

<tr>
  <td>

  **Structured Field**

  (Complex Object)

  </td>
  <td>

  ```

  #+Message#
    #Greeting#, #Name# !!!
  #+#

  ```

  </td>
  <td>
      
  ```json
  { "Message": { 
      "Greeting": "Hello",
      "Name": "HashScript"
    }
  }
  ```
  </td>
  <td>
      
  ```
 
 
  Hello, HashScript
  

  ```
  </td>
</tr>

<tr>
  <td>

  **Structured Field**
  
  (Collection)

  </td>
  <td>

  ```

  #+Languages#
    - #Name# : #Year#
  #+#

  ```

  </td>
  <td>
      
  ```json
  { "Languages": [
    { "Name": "HZ", "Year": "2022" },
    { "Name": "VB", "Year": "1964" },
    { "Name": "CS", "Year": "2000" }
  ]}
  ```
  </td>
  <td>
      
  ```
 
  - HZ: 2022
  - VB: 1964
  - CS: 2000
  
  ```
  </td>
</tr>

<tr>
  <td>

  **Conditional Field**

  (Boolean)
  </td>
  <td>

  ```
  Hi
  #?IsDoctor#Dr.#?#
  #!IsDoctor#Sr.#!#
  #Name#
  ```

  </td>
  <td>
      
  ```json
  { 
    "IsDoctor": false,
    "Name": "Denis"
  }
  ```
  </td>
  <td>
      
  ```
  Hi

  Sr.
  Denis
  ```
  </td>
</tr>

<tr>
  <td>

  **Conditional Field**

  (Text, Number)
  </td>
  <td>

  ```

  Name: #Name#
  #?Address# Adress: #Address# #?#
  #?Email# E-mail: #Email# #?#
  #?Posts# Replies: #Posts# #?#

  ```

  </td>
  <td>
      
  ```json
  { 
    "Name": "Denis",
    "Email": "denis@hashscript.org",
    "Address": "",
    "Posts": 10
  }
  ```
  </td>
  <td>
      
  ```

  Name: Denis
    
  E-mail: denis@hashscript.org
  Replies: 10

  ```
  </td>
</tr>

<tr>
  <td>

  **Conditional Field**

  (Collection)
  </td>
  <td>

  ```

  #!Messages# Empty Inbox #!#

  ```

  </td>
  <td>
      
  ```json
  { 
    "Messages": []
  }
  ```
  </td>
  <td>
      
  ```

  Empty Inbox

  ```
  </td>
</tr>

<tr>
  <td>

  **Conditional Field**

  (Value)
  </td>
  <td>

  ```

  #!Value# There is no value #!#

  ```

  </td>
  <td>
      
  ```json
  { 
    "Value": null
  }
  ```
  </td>
  <td>
      
  ```

  There is no value

  ```
  </td>
</tr>

<tr>
  <td>

  **Special Function**

  </td>
  <td>

  ```

  #+Languages#
    #Name# #!.Last# > #!#
  #+#

  ```

  </td>
  <td>
      
  ```json
  { "Languages": [
    { "Name": "HZ" },
    { "Name": "VB" },
    { "Name": "CS" }
  ]}
  ```
  </td>
  <td>
      
  ```


  HZ > VB > CS


  ```
  </td>
</tr>

</table>
import React from 'react';
import { FieldRenderProps } from 'react-final-form';
import { FormFieldProps, Label, Form } from 'semantic-ui-react';

interface IProps
  extends FieldRenderProps<string, HTMLElement>,
    FormFieldProps {}

const TextInput: React.FC<IProps> = ({
  input,
  width,
  name,
  type,
  placeholder,
  meta: { touched, error },
}) => {
  return (
    <Form.Field
      name={name}
      type={type}
      error={touched && !!error}
      width={width}
    >
      <input placeholder={placeholder} {...input} />
      {touched && error && (
        <Label basic color="red">
          {error}
        </Label>
      )}
    </Form.Field>
  );
};

export default TextInput;

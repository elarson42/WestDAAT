import globals from 'globals';
import pluginJs from '@eslint/js';
import tseslint from 'typescript-eslint';
import pluginReact from 'eslint-plugin-react';

export default [
  {
    files: ['**/*.{js,mjs,cjs,ts,jsx,tsx}'],
  },
  {
    languageOptions: {
      globals: globals.browser,
    },
    settings: {
      react: {
        version: 'detect',
      },
    },
  },
  pluginJs.configs.recommended,
  ...tseslint.configs.recommended,
  pluginReact.configs.flat.recommended,
  {
    rules: {
      // Remove this to get a handle of the "any" violations
      '@typescript-eslint/no-explicit-any': 'off',

      // Remove this to get a handle of using empty object violations
      '@typescript-eslint/no-empty-object-type': 'off',

      // Remove this to get a handle of unused variables
      '@typescript-eslint/no-unused-vars': 'off',

      // Remove this to get a handle of the missing "key" prop violations
      'react/jsx-key': 'off',

      // Remove this to get a handle of unescaped single quotes in template strings
      'react/no-unescaped-entities': 'off',
    },
  },
];
